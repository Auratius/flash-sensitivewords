using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
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
    }
}
