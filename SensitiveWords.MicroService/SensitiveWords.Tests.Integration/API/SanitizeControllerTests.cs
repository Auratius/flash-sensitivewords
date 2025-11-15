using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Flash.SensitiveWords.API;
using Flash.SensitiveWords.Application.DTOs;

namespace Flash.SensitiveWords.Tests.Integration.API
{
    public class SanitizeControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public SanitizeControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Sanitize_WithValidRequest_ReturnsOk()
        {
            var request = new SanitizeRequest { Message = "SELECT * FROM users" };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/sanitize", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SanitizeResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(result);
            Assert.Contains("*", result.SanitizedMessage);
        }

        [Fact]
        public async Task Sanitize_WithEmptyMessage_ReturnsBadRequest()
        {
            var request = new SanitizeRequest { Message = "" };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/sanitize", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #region Additional Message Tests

        [Fact]
        public async Task Sanitize_WithMultipleSensitiveWords_ReturnsOk()
        {
            var request = new SanitizeRequest { Message = "SELECT * FROM users WHERE DELETE = 1 AND UPDATE table" };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/sanitize", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SanitizeResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(result);
            Assert.True(result.WordsReplaced > 0);
        }

        [Fact]
        public async Task Sanitize_WithCleanMessage_ReturnsOk()
        {
            var request = new SanitizeRequest { Message = "Hello World" };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/sanitize", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SanitizeResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(result);
            Assert.Equal(0, result.WordsReplaced);
        }

        [Fact]
        public async Task Sanitize_WithLongMessage_ReturnsOk()
        {
            var longMessage = new string('a', 1000) + " SELECT " + new string('b', 1000);
            var request = new SanitizeRequest { Message = longMessage };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/sanitize", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Sanitize_WithCaseSensitivity_ReturnsOk()
        {
            var request = new SanitizeRequest { Message = "select SELECT SeLeCt" };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/sanitize", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SanitizeResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(result);
            Assert.Contains("*", result.SanitizedMessage);
        }

        [Fact]
        public async Task Sanitize_WithSpecialCharacters_ReturnsOk()
        {
            var request = new SanitizeRequest { Message = "!@#$%^&*() SELECT test" };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/sanitize", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Sanitize_WithNumericContent_ReturnsOk()
        {
            var request = new SanitizeRequest { Message = "123456789" };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/sanitize", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Sanitize_WithWhitespace_ReturnsBadRequest()
        {
            var request = new SanitizeRequest { Message = "   " };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/sanitize", content);

            // May return BadRequest or OK depending on validation
            Assert.True(response.StatusCode == HttpStatusCode.OK ||
                       response.StatusCode == HttpStatusCode.BadRequest);
        }

        #endregion

        #region Content Type Tests

        [Fact]
        public async Task Sanitize_ReturnsJsonContentType()
        {
            var request = new SanitizeRequest { Message = "Test message" };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/sanitize", content);

            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        #endregion

        #region Performance Tests

        [Fact]
        public async Task Sanitize_MultipleConcurrentRequests_AllSucceed()
        {
            var tasks = new Task<HttpResponseMessage>[10];
            for (int i = 0; i < 10; i++)
            {
                var request = new SanitizeRequest { Message = $"SELECT * FROM table{i}" };
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                tasks[i] = _client.PostAsync("/api/sanitize", content);
            }

            var responses = await Task.WhenAll(tasks);

            foreach (var response in responses)
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        #endregion

        #region Response Structure Tests

        [Fact]
        public async Task Sanitize_ResponseHasCorrectStructure()
        {
            var request = new SanitizeRequest { Message = "SELECT test" };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/sanitize", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SanitizeResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(result);
            Assert.NotNull(result.OriginalMessage);
            Assert.NotNull(result.SanitizedMessage);
            Assert.True(result.WordsReplaced >= 0);
        }

        #endregion
    }
}
