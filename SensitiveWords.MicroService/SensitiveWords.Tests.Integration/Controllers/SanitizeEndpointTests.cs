using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Flash.SensitiveWords.Application.DTOs;

namespace Flash.SensitiveWords.Tests.Integration.Controllers
{
    public class SanitizeEndpointTests : IClassFixture<WebApplicationFactory<Flash.SensitiveWords.API.Program>>
    {
        private readonly WebApplicationFactory<Flash.SensitiveWords.API.Program> _factory;

        public SanitizeEndpointTests(WebApplicationFactory<Flash.SensitiveWords.API.Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_Sanitize_ReturnsSanitizedResponse()
        {
            var client = _factory.CreateClient();

            var request = new SanitizeRequest { Message = "SELECT * FROM users" };

            var response = await client.PostAsJsonAsync("/api/sanitize", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<SanitizeResponse>();
            Assert.NotNull(body);
            Assert.Equal(request.Message, body.OriginalMessage);
            Assert.False(string.IsNullOrWhiteSpace(body.SanitizedMessage));
            Assert.True(body.WordsReplaced >= 1);
        }

        [Fact]
        public async Task Post_Sanitize_WithInvalidModel_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();
            // Send empty body to trigger model validation
            var response = await client.PostAsJsonAsync("/api/sanitize", new { });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}