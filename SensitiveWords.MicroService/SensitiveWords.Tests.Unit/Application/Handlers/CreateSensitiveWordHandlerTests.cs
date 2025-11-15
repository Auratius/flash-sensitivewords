using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Flash.SensitiveWords.Application.Commands;
using Flash.SensitiveWords.Application.Handlers;
using Flash.SensitiveWords.Domain.Entities;
using Flash.SensitiveWords.Domain.Interfaces;

namespace Flash.SensitiveWords.Tests.Unit.Application.Handlers
{
    public class CreateSensitiveWordHandlerTests
    {
        private readonly Mock<ISensitiveWordRepository> _mockRepo;
        private readonly Mock<IOperationStatsRepository> _mockStats;
        private readonly CreateSensitiveWordHandler _handler;

        public CreateSensitiveWordHandlerTests()
        {
            _mockRepo = new Mock<ISensitiveWordRepository>();
            _mockStats = new Mock<IOperationStatsRepository>();
            _handler = new CreateSensitiveWordHandler(_mockRepo.Object, _mockStats.Object);
        }

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CreateSensitiveWordHandler(null!, _mockStats.Object));
        }

        [Fact]
        public async Task HandleAsync_WithValidCommand_CreatesAndTracks()
        {
            var command = new CreateSensitiveWordCommand("TRUNCATE");
            var expectedId = Guid.NewGuid();

            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<SensitiveWord>())).ReturnsAsync(expectedId);
            _mockStats.Setup(s => s.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.HandleAsync(command);

            Assert.Equal(expectedId, result);
            _mockRepo.Verify(r => r.CreateAsync(It.Is<SensitiveWord>(w => w.Word == "TRUNCATE")), Times.Once);
            _mockStats.Verify(s => s.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithExistingWord_ThrowsInvalidOperationException()
        {
            var command = new CreateSensitiveWordCommand("SELECT");
            _mockRepo.Setup(r => r.ExistsAsync("SELECT")).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
            _mockRepo.Verify(r => r.CreateAsync(It.IsAny<SensitiveWord>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WithNullCommand_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.HandleAsync(null!));
        }

        [Fact]
        public async Task HandleAsync_WhenCreateFails_ThrowsInvalidOperationException()
        {
            var command = new CreateSensitiveWordCommand("TRUNCATE");

            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<SensitiveWord>())).ThrowsAsync(new Exception("db error"));

            await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
            _mockStats.Verify(s => s.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
