using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Flash.SensitiveWords.Application.Handlers;
using Flash.SensitiveWords.Application.Queries;
using Flash.SensitiveWords.Domain.Services;
using Flash.SensitiveWords.Domain.ValueObjects;
using Flash.SensitiveWords.Domain.Interfaces;

namespace Flash.SensitiveWords.Tests.Unit.Application.Handlers
{
    public class SanitizeMessageHandlerTests
    {
        private readonly Mock<ISanitizationService> _mockService;
        private readonly Mock<IOperationStatsRepository> _mockStatsRepo;
        private readonly SanitizeMessageHandler _handler;

        public SanitizeMessageHandlerTests()
        {
            _mockService = new Mock<ISanitizationService>();
            _mockStatsRepo = new Mock<IOperationStatsRepository>();
            _handler = new SanitizeMessageHandler(_mockService.Object, _mockStatsRepo.Object);
        }

        [Fact]
        public void Constructor_WithNullService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SanitizeMessageHandler(null!, _mockStatsRepo.Object));
        }

        [Fact]
        public void Constructor_WithNullStatsRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SanitizeMessageHandler(_mockService.Object, null!));
        }

        [Fact]
        public async Task HandleAsync_WithValidQuery_ReturnsSanitizedResponse()
        {
            var query = new SanitizeMessageQuery("SELECT * FROM users");
            var sanitizedMessage = new SanitizedMessage("SELECT * FROM users", "****** * FROM users", 1);

            _mockService
                .Setup(s => s.SanitizeMessageAsync("SELECT * FROM users"))
                .ReturnsAsync(sanitizedMessage);

            _mockStatsRepo
                .Setup(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.HandleAsync(query);

            Assert.Equal("SELECT * FROM users", result.OriginalMessage);
            Assert.Equal("****** * FROM users", result.SanitizedMessage);
            Assert.Equal(1, result.WordsReplaced);

            _mockService.Verify(s => s.SanitizeMessageAsync("SELECT * FROM users"), Times.Once);
            _mockStatsRepo.Verify(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithNullQuery_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.HandleAsync(null!));
        }
    }
}
