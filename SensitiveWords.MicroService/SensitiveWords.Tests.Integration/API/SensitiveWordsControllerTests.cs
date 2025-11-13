using System;
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
    public class SensitiveWordsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public SensitiveWordsControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/sensitivewords");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_WithValidRequest_ReturnsCreated()
        {
            var request = new CreateSensitiveWordRequest { Word = $"TESTWORD{Guid.NewGuid()}" };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/sensitivewords", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Create_WithEmptyWord_ReturnsBadRequest()
        {
            var request = new CreateSensitiveWordRequest { Word = "" };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/sensitivewords", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            var invalidId = Guid.NewGuid();

            var response = await _client.GetAsync($"/api/sensitivewords/{invalidId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Update_WithValidRequest_ReturnsNoContent()
        {
            // First create a word
            var uniqueWord = $"UPDATETEST{Guid.NewGuid()}";
            var createRequest = new CreateSensitiveWordRequest { Word = uniqueWord };
            var createContent = new StringContent(JsonSerializer.Serialize(createRequest), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/sensitivewords", createContent);
            var createResponseBody = await createResponse.Content.ReadAsStringAsync();
            var createdResult = JsonSerializer.Deserialize<JsonElement>(createResponseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var createdId = createdResult.GetProperty("id").GetGuid();

            // Now update it with a new unique word
            var updatedWord = $"UPDATED{Guid.NewGuid()}";
            var updateRequest = new UpdateSensitiveWordRequest { Word = updatedWord, IsActive = false };
            var updateContent = new StringContent(JsonSerializer.Serialize(updateRequest), Encoding.UTF8, "application/json");
            var updateResponse = await _client.PutAsync($"/api/sensitivewords/{createdId}", updateContent);

            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        }

        [Fact]
        public async Task Update_WithInvalidId_ReturnsBadRequest()
        {
            var invalidId = Guid.NewGuid();
            var updateRequest = new UpdateSensitiveWordRequest { Word = "UPDATED", IsActive = true };
            var content = new StringContent(JsonSerializer.Serialize(updateRequest), Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/api/sensitivewords/{invalidId}", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_WithEmptyWord_ReturnsBadRequest()
        {
            var updateRequest = new UpdateSensitiveWordRequest { Word = "", IsActive = true };
            var content = new StringContent(JsonSerializer.Serialize(updateRequest), Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/api/sensitivewords/{Guid.NewGuid()}", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent()
        {
            // First create a word
            var createRequest = new CreateSensitiveWordRequest { Word = $"DELETETEST{Guid.NewGuid()}" };
            var createContent = new StringContent(JsonSerializer.Serialize(createRequest), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/sensitivewords", createContent);
            var createResponseBody = await createResponse.Content.ReadAsStringAsync();
            var createdResult = JsonSerializer.Deserialize<JsonElement>(createResponseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var createdId = createdResult.GetProperty("id").GetGuid();

            // Now delete it
            var deleteResponse = await _client.DeleteAsync($"/api/sensitivewords/{createdId}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_WithInvalidId_ReturnsBadRequest()
        {
            var invalidId = Guid.NewGuid();

            var response = await _client.DeleteAsync($"/api/sensitivewords/{invalidId}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
