using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Flash.SensitiveWords.API;
using Flash.SensitiveWords.Domain.Entities;

namespace Flash.SensitiveWords.Tests.Integration.API
{
    public class StatisticsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public StatisticsControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAllStatistics_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/statistics");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
        }

        [Fact]
        public async Task GetAllStatistics_ReturnsJsonArray()
        {
            // Act
            var response = await _client.GetAsync("/api/statistics");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var stats = JsonSerializer.Deserialize<OperationStat[]>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.NotNull(stats);
        }

        [Fact]
        public async Task GetAllStatistics_ReturnsCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/api/statistics");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        #endregion

        #region GetByType Tests

        [Fact]
        public async Task GetStatisticsByType_WithValidType_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/statistics/CREATE");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
        }

        [Fact]
        public async Task GetStatisticsByType_WithReadType_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/statistics/READ");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetStatisticsByType_WithUpdateType_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/statistics/UPDATE");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetStatisticsByType_WithDeleteType_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/statistics/DELETE");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetStatisticsByType_WithSanitizeType_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/statistics/SANITIZE");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetStatisticsByType_WithLowercaseType_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/statistics/create");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetStatisticsByType_WithMixedCaseType_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/statistics/CrEaTe");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetStatisticsByType_WithInvalidType_ReturnsOk()
        {
            // Even invalid types should return OK with empty results
            // Act
            var response = await _client.GetAsync("/api/statistics/INVALID");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var stats = JsonSerializer.Deserialize<OperationStat[]>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.NotNull(stats);
        }

        [Fact]
        public async Task GetStatisticsByType_ReturnsJsonArray()
        {
            // Act
            var response = await _client.GetAsync("/api/statistics/CREATE");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var stats = JsonSerializer.Deserialize<OperationStat[]>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.NotNull(stats);
        }

        #endregion

        #region Reset Tests

        [Fact]
        public async Task ResetStatistics_ReturnsOk()
        {
            // Act
            var response = await _client.PostAsync("/api/statistics/reset", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("reset", content.ToLower());
        }

        [Fact]
        public async Task ResetStatistics_ReturnsJsonResponse()
        {
            // Act
            var response = await _client.PostAsync("/api/statistics/reset", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.True(result.TryGetProperty("message", out var message));
            Assert.Contains("reset", message.GetString()?.ToLower());
        }

        [Fact]
        public async Task ResetStatistics_ReturnsCorrectContentType()
        {
            // Act
            var response = await _client.PostAsync("/api/statistics/reset", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        #endregion

        #region End-to-End Workflow Tests

        [Fact]
        public async Task StatisticsWorkflow_GetAllThenGetByTypeThenReset_WorksCorrectly()
        {
            // Step 1: Get all statistics
            var getAllResponse = await _client.GetAsync("/api/statistics");
            Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);
            var allStatsContent = await getAllResponse.Content.ReadAsStringAsync();
            var allStats = JsonSerializer.Deserialize<OperationStat[]>(allStatsContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.NotNull(allStats);

            // Step 2: Get statistics by type
            var getByTypeResponse = await _client.GetAsync("/api/statistics/CREATE");
            Assert.Equal(HttpStatusCode.OK, getByTypeResponse.StatusCode);

            // Step 3: Reset statistics
            var resetResponse = await _client.PostAsync("/api/statistics/reset", null);
            Assert.Equal(HttpStatusCode.OK, resetResponse.StatusCode);

            // Step 4: Verify stats still accessible after reset
            var getAllAfterResetResponse = await _client.GetAsync("/api/statistics");
            Assert.Equal(HttpStatusCode.OK, getAllAfterResetResponse.StatusCode);
        }

        [Fact]
        public async Task GetStatisticsByType_ForAllOperationTypes_ReturnsOk()
        {
            // Test all valid operation types
            var operationTypes = new[] { "CREATE", "READ", "UPDATE", "DELETE", "SANITIZE" };

            foreach (var operationType in operationTypes)
            {
                var response = await _client.GetAsync($"/api/statistics/{operationType}");
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();
                var stats = JsonSerializer.Deserialize<OperationStat[]>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                Assert.NotNull(stats);
            }
        }

        #endregion

        #region Concurrent Access Tests

        [Fact]
        public async Task GetAllStatistics_ConcurrentRequests_AllSucceed()
        {
            // Act - Make multiple concurrent requests
            var tasks = new Task<HttpResponseMessage>[10];
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = _client.GetAsync("/api/statistics");
            }

            var responses = await Task.WhenAll(tasks);

            // Assert - All requests should succeed
            foreach (var response in responses)
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task GetStatisticsByType_ConcurrentRequests_AllSucceed()
        {
            // Act - Make multiple concurrent requests for different types
            var tasks = new[]
            {
                _client.GetAsync("/api/statistics/CREATE"),
                _client.GetAsync("/api/statistics/READ"),
                _client.GetAsync("/api/statistics/UPDATE"),
                _client.GetAsync("/api/statistics/DELETE"),
                _client.GetAsync("/api/statistics/SANITIZE")
            };

            var responses = await Task.WhenAll(tasks);

            // Assert - All requests should succeed
            foreach (var response in responses)
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        #endregion

        #region Response Validation Tests

        [Fact]
        public async Task GetAllStatistics_ResponseHasValidStructure()
        {
            // Act
            var response = await _client.GetAsync("/api/statistics");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var stats = JsonSerializer.Deserialize<OperationStat[]>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(stats);
            // If there are stats, validate their structure
            if (stats.Length > 0)
            {
                foreach (var stat in stats)
                {
                    Assert.True(stat.Id > 0);
                    Assert.False(string.IsNullOrEmpty(stat.OperationType));
                    Assert.False(string.IsNullOrEmpty(stat.ResourceType));
                    Assert.True(stat.Count >= 0);
                    Assert.NotEqual(default(DateTime), stat.LastUpdated);
                }
            }
        }

        [Fact]
        public async Task GetStatisticsByType_ResponseHasValidStructure()
        {
            // Act
            var response = await _client.GetAsync("/api/statistics/CREATE");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var stats = JsonSerializer.Deserialize<OperationStat[]>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(stats);
            // If there are stats, validate they match the requested type
            if (stats.Length > 0)
            {
                foreach (var stat in stats)
                {
                    Assert.Equal("CREATE", stat.OperationType.ToUpper());
                }
            }
        }

        #endregion
    }
}
