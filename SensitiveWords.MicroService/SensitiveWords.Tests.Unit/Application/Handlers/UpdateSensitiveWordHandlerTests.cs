using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Flash.SensitiveWords.Application.Commands;
using Flash.SensitiveWords.Application.Handlers;
using Flash.SensitiveWords.Domain.Entities;
using Flash.SensitiveWords.Domain.Interfaces;

namespace Flash.SensitiveWords.Tests.Unit.Application.Handlers
{
    public class UpdateSensitiveWordHandlerTests
    {
        private readonly Mock<ISensitiveWordRepository> _mockRepository;
        private readonly UpdateSensitiveWordHandler _handler;

        public UpdateSensitiveWordHandlerTests()
        {
            _mockRepository = new Mock<ISensitiveWordRepository>();
            _handler = new UpdateSensitiveWordHandler(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new UpdateSensitiveWordHandler(null!));
        }

        [Fact]
        public async Task HandleAsync_WithValidCommand_UpdatesWord()
        {
            var wordId = Guid.NewGuid();
            var existingWord = new SensitiveWord(wordId, "SELECT", true, DateTime.UtcNow, DateTime.UtcNow);
            var command = new UpdateSensitiveWordCommand(wordId, "DROP", true);

            _mockRepository.Setup(r => r.GetByIdAsync(wordId)).ReturnsAsync(existingWord);
            _mockRepository.Setup(r => r.GetByWordAsync("DROP")).ReturnsAsync((SensitiveWord?)null);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<SensitiveWord>())).ReturnsAsync(true);

            var result = await _handler.HandleAsync(command);

            Assert.True(result);
            Assert.Equal("DROP", existingWord.Word);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<SensitiveWord>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithDeactivation_DeactivatesWord()
        {
            var wordId = Guid.NewGuid();
            var existingWord = new SensitiveWord(wordId, "SELECT", true, DateTime.UtcNow, DateTime.UtcNow);
            var command = new UpdateSensitiveWordCommand(wordId, "SELECT", false);

            _mockRepository.Setup(r => r.GetByIdAsync(wordId)).ReturnsAsync(existingWord);
            _mockRepository.Setup(r => r.GetByWordAsync("SELECT")).ReturnsAsync(existingWord);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<SensitiveWord>())).ReturnsAsync(true);

            var result = await _handler.HandleAsync(command);

            Assert.True(result);
            Assert.False(existingWord.IsActive);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<SensitiveWord>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithActivation_ActivatesWord()
        {
            var wordId = Guid.NewGuid();
            var existingWord = new SensitiveWord(wordId, "SELECT", false, DateTime.UtcNow, DateTime.UtcNow);
            var command = new UpdateSensitiveWordCommand(wordId, "SELECT", true);

            _mockRepository.Setup(r => r.GetByIdAsync(wordId)).ReturnsAsync(existingWord);
            _mockRepository.Setup(r => r.GetByWordAsync("SELECT")).ReturnsAsync(existingWord);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<SensitiveWord>())).ReturnsAsync(true);

            var result = await _handler.HandleAsync(command);

            Assert.True(result);
            Assert.True(existingWord.IsActive);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<SensitiveWord>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithNonExistentWord_ThrowsInvalidOperationException()
        {
            var wordId = Guid.NewGuid();
            var command = new UpdateSensitiveWordCommand(wordId, "DROP", true);

            _mockRepository.Setup(r => r.GetByIdAsync(wordId)).ReturnsAsync((SensitiveWord?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<SensitiveWord>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WithDuplicateWord_ThrowsInvalidOperationException()
        {
            var wordId = Guid.NewGuid();
            var otherWordId = Guid.NewGuid();
            var existingWord = new SensitiveWord(wordId, "SELECT", true, DateTime.UtcNow, DateTime.UtcNow);
            var duplicateWord = new SensitiveWord(otherWordId, "DROP", true, DateTime.UtcNow, DateTime.UtcNow);
            var command = new UpdateSensitiveWordCommand(wordId, "DROP", true);

            _mockRepository.Setup(r => r.GetByIdAsync(wordId)).ReturnsAsync(existingWord);
            _mockRepository.Setup(r => r.GetByWordAsync("DROP")).ReturnsAsync(duplicateWord);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<SensitiveWord>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WithNullCommand_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.HandleAsync(null!));
        }
    }
}
