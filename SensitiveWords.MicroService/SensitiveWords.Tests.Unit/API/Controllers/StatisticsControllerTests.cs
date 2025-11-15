using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SensitiveWords.API.Controllers;
using Flash.SensitiveWords.Domain.Entities;
using Flash.SensitiveWords.Domain.Interfaces;

namespace Flash.SensitiveWords.Tests.Unit.API.Controllers
{
    public class StatisticsControllerTests
    {
        private readonly Mock<IOperationStatsRepository> _mockStatsRepository;
        private readonly Mock<ILogger<StatisticsController>> _mockLogger;
        private readonly StatisticsController _controller;

        public StatisticsControllerTests()
        {
            _mockStatsRepository = new Mock<IOperationStatsRepository>();
            _mockLogger = new Mock<ILogger<StatisticsController>>();
            _controller = new StatisticsController(_mockStatsRepository.Object, _mockLogger.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullStatsRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new StatisticsController(null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new StatisticsController(_mockStatsRepository.Object, null!));
        }

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            var controller = new StatisticsController(_mockStatsRepository.Object, _mockLogger.Object);
            Assert.NotNull(controller);
        }

        #endregion

        #region GetAllStatistics Tests

        [Fact]
        public async Task GetAllStatistics_WithExistingStats_ReturnsOkWithStats()
        {
            // Arrange
            var stats = new List<OperationStat>
            {
                new OperationStat
                {
                    Id = 1,
                    OperationType = OperationType.Create,
                    ResourceType = ResourceType.SensitiveWord,
                    Count = 10,
                    LastUpdated = DateTime.UtcNow
                },
                new OperationStat
                {
                    Id = 2,
                    OperationType = OperationType.Sanitize,
                    ResourceType = ResourceType.Message,
                    Count = 25,
                    LastUpdated = DateTime.UtcNow
                }
            };

            _mockStatsRepository
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(stats);

            // Act
            var result = await _controller.GetAllStatistics(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStats = Assert.IsAssignableFrom<IEnumerable<OperationStat>>(okResult.Value);
            Assert.Equal(2, returnedStats.Count());
            _mockStatsRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllStatistics_WithNoStats_ReturnsOkWithEmptyList()
        {
            // Arrange
            var stats = new List<OperationStat>();

            _mockStatsRepository
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(stats);

            // Act
            var result = await _controller.GetAllStatistics(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStats = Assert.IsAssignableFrom<IEnumerable<OperationStat>>(okResult.Value);
            Assert.Empty(returnedStats);
        }

        [Fact]
        public async Task GetAllStatistics_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockStatsRepository
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAllStatistics(CancellationToken.None);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetAllStatistics_WhenCancelled_ReturnsInternalServerError()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockStatsRepository
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException());

            // Act
            var result = await _controller.GetAllStatistics(cts.Token);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion

        #region GetStatisticsByType Tests

        [Fact]
        public async Task GetStatisticsByType_WithValidType_ReturnsOkWithStats()
        {
            // Arrange
            var stats = new List<OperationStat>
            {
                new OperationStat
                {
                    Id = 1,
                    OperationType = OperationType.Create,
                    ResourceType = ResourceType.SensitiveWord,
                    Count = 10,
                    LastUpdated = DateTime.UtcNow
                }
            };

            _mockStatsRepository
                .Setup(x => x.GetByTypeAsync("CREATE", It.IsAny<CancellationToken>()))
                .ReturnsAsync(stats);

            // Act
            var result = await _controller.GetStatisticsByType("CREATE", CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStats = Assert.IsAssignableFrom<IEnumerable<OperationStat>>(okResult.Value);
            Assert.Single(returnedStats);
            _mockStatsRepository.Verify(x => x.GetByTypeAsync("CREATE", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetStatisticsByType_WithLowercaseType_ConvertsToUppercase()
        {
            // Arrange
            var stats = new List<OperationStat>
            {
                new OperationStat
                {
                    Id = 1,
                    OperationType = OperationType.Read,
                    ResourceType = ResourceType.SensitiveWord,
                    Count = 5,
                    LastUpdated = DateTime.UtcNow
                }
            };

            _mockStatsRepository
                .Setup(x => x.GetByTypeAsync("READ", It.IsAny<CancellationToken>()))
                .ReturnsAsync(stats);

            // Act
            var result = await _controller.GetStatisticsByType("read", CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockStatsRepository.Verify(x => x.GetByTypeAsync("READ", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetStatisticsByType_WithEmptyType_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetStatisticsByType("", CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task GetStatisticsByType_WithNullType_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetStatisticsByType(null!, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task GetStatisticsByType_WithWhitespaceType_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetStatisticsByType("   ", CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task GetStatisticsByType_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockStatsRepository
                .Setup(x => x.GetByTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetStatisticsByType("CREATE", CancellationToken.None);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetStatisticsByType_WithNoMatchingStats_ReturnsOkWithEmptyList()
        {
            // Arrange
            var stats = new List<OperationStat>();

            _mockStatsRepository
                .Setup(x => x.GetByTypeAsync("DELETE", It.IsAny<CancellationToken>()))
                .ReturnsAsync(stats);

            // Act
            var result = await _controller.GetStatisticsByType("DELETE", CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStats = Assert.IsAssignableFrom<IEnumerable<OperationStat>>(okResult.Value);
            Assert.Empty(returnedStats);
        }

        #endregion

        #region ResetStatistics Tests

        [Fact]
        public async Task ResetStatistics_WithSuccessfulReset_ReturnsOkWithMessage()
        {
            // Arrange
            _mockStatsRepository
                .Setup(x => x.ResetAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ResetStatistics(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockStatsRepository.Verify(x => x.ResetAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ResetStatistics_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockStatsRepository
                .Setup(x => x.ResetAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.ResetStatistics(CancellationToken.None);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task ResetStatistics_WhenCalled_LogsWarning()
        {
            // Arrange
            _mockStatsRepository
                .Setup(x => x.ResetAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _controller.ResetStatistics(CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("reset")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ResetStatistics_WhenCancelled_ReturnsInternalServerError()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockStatsRepository
                .Setup(x => x.ResetAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException());

            // Act
            var result = await _controller.ResetStatistics(cts.Token);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Fact]
        public async Task GetAllStatistics_WithLargeDataset_ReturnsAllStats()
        {
            // Arrange
            var stats = Enumerable.Range(1, 100).Select(i => new OperationStat
            {
                Id = i,
                OperationType = OperationType.Read,
                ResourceType = ResourceType.SensitiveWord,
                Count = i * 10,
                LastUpdated = DateTime.UtcNow.AddMinutes(-i)
            }).ToList();

            _mockStatsRepository
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(stats);

            // Act
            var result = await _controller.GetAllStatistics(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStats = Assert.IsAssignableFrom<IEnumerable<OperationStat>>(okResult.Value);
            Assert.Equal(100, returnedStats.Count());
        }

        [Fact]
        public async Task GetStatisticsByType_WithMultipleOperationTypes_ReturnsCorrectResults()
        {
            // Arrange
            var createStats = new List<OperationStat>
            {
                new OperationStat
                {
                    Id = 1,
                    OperationType = OperationType.Create,
                    ResourceType = ResourceType.SensitiveWord,
                    Count = 10,
                    LastUpdated = DateTime.UtcNow
                }
            };

            var sanitizeStats = new List<OperationStat>
            {
                new OperationStat
                {
                    Id = 2,
                    OperationType = OperationType.Sanitize,
                    ResourceType = ResourceType.Message,
                    Count = 25,
                    LastUpdated = DateTime.UtcNow
                }
            };

            _mockStatsRepository
                .Setup(x => x.GetByTypeAsync("CREATE", It.IsAny<CancellationToken>()))
                .ReturnsAsync(createStats);

            _mockStatsRepository
                .Setup(x => x.GetByTypeAsync("SANITIZE", It.IsAny<CancellationToken>()))
                .ReturnsAsync(sanitizeStats);

            // Act
            var createResult = await _controller.GetStatisticsByType("CREATE", CancellationToken.None);
            var sanitizeResult = await _controller.GetStatisticsByType("SANITIZE", CancellationToken.None);

            // Assert
            var createOkResult = Assert.IsType<OkObjectResult>(createResult);
            var createReturnedStats = Assert.IsAssignableFrom<IEnumerable<OperationStat>>(createOkResult.Value);
            Assert.Single(createReturnedStats);
            Assert.Equal(OperationType.Create, createReturnedStats.First().OperationType);

            var sanitizeOkResult = Assert.IsType<OkObjectResult>(sanitizeResult);
            var sanitizeReturnedStats = Assert.IsAssignableFrom<IEnumerable<OperationStat>>(sanitizeOkResult.Value);
            Assert.Single(sanitizeReturnedStats);
            Assert.Equal(OperationType.Sanitize, sanitizeReturnedStats.First().OperationType);
        }

        #endregion
    }
}
