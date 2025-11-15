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
    public class DeleteSensitiveWordHandlerTests
    {
        private readonly Mock<ISensitiveWordRepository> _mockRepository;
        private readonly Mock<IOperationStatsRepository> _mockStatsRepo;
        private readonly DeleteSensitiveWordHandler _handler;

        public DeleteSensitiveWordHandlerTests()
        {
            _mockRepository = new Mock<ISensitiveWordRepository>();
            _mockStatsRepo = new Mock<IOperationStatsRepository>();
            _handler = new DeleteSensitiveWordHandler(_mockRepository.Object, _mockStatsRepo.Object);
        }

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DeleteSensitiveWordHandler(null!, _mockStatsRepo.Object));
        }

        [Fact]
        public void Constructor_WithNullStatsRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DeleteSensitiveWordHandler(_mockRepository.Object, null!));
        }

        [Fact]
        public async Task HandleAsync_WithValidCommand_DeletesWord()
        {
            var wordId = Guid.NewGuid();
            var existingWord = new SensitiveWord(wordId, "SELECT", true, DateTime.UtcNow, DateTime.UtcNow);
            var command = new DeleteSensitiveWordCommand(wordId);

            _mockRepository.Setup(r => r.GetByIdAsync(wordId)).ReturnsAsync(existingWord);
            _mockRepository.Setup(r => r.DeleteAsync(wordId)).ReturnsAsync(true);
            _mockStatsRepo
                .Setup(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.HandleAsync(command);

            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteAsync(wordId), Times.Once);
            _mockStatsRepo.Verify(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithNonExistentWord_ThrowsInvalidOperationException()
        {
            var wordId = Guid.NewGuid();
            var existingWord = new SensitiveWord(wordId, "SELECT", true, DateTime.UtcNow, DateTime.UtcNow);
            var command = new DeleteSensitiveWordCommand(wordId);

            _mockRepository.Setup(r => r.GetByIdAsync(wordId)).ReturnsAsync(existingWord);
            _mockRepository.Setup(r => r.DeleteAsync(wordId)).ReturnsAsync(true);
            _mockStatsRepo
                .Setup(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.HandleAsync(command);

            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteAsync(wordId), Times.Once);
            _mockStatsRepo.Verify(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithNullCommand_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.HandleAsync(null!));
            _mockRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
            _mockStatsRepo.Verify(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDeleteFails_ReturnsFalse()
        {
            var wordId = Guid.NewGuid();
            var existingWord = new SensitiveWord(wordId, "SELECT", true, DateTime.UtcNow, DateTime.UtcNow);
            var command = new DeleteSensitiveWordCommand(wordId);

            _mockRepository.Setup(r => r.GetByIdAsync(wordId)).ReturnsAsync(existingWord);
            _mockRepository.Setup(r => r.DeleteAsync(wordId)).ReturnsAsync(false);
            _mockStatsRepo
                .Setup(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.HandleAsync(command);

            Assert.False(result);
            _mockRepository.Verify(r => r.DeleteAsync(wordId), Times.Once);
            _mockStatsRepo.Verify(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
