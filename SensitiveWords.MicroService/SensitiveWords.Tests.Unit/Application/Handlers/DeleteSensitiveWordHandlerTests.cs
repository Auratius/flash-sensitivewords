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
    public class DeleteSensitiveWordHandlerTests
    {
        private readonly Mock<ISensitiveWordRepository> _mockRepository;
        private readonly DeleteSensitiveWordHandler _handler;

        public DeleteSensitiveWordHandlerTests()
        {
            _mockRepository = new Mock<ISensitiveWordRepository>();
            _handler = new DeleteSensitiveWordHandler(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DeleteSensitiveWordHandler(null!));
        }

        [Fact]
        public async Task HandleAsync_WithValidCommand_DeletesWord()
        {
            var wordId = Guid.NewGuid();
            var existingWord = new SensitiveWord("SELECT");
            var command = new DeleteSensitiveWordCommand(wordId);

            _mockRepository.Setup(r => r.GetByIdAsync(wordId)).ReturnsAsync(existingWord);
            _mockRepository.Setup(r => r.DeleteAsync(wordId)).ReturnsAsync(true);

            var result = await _handler.HandleAsync(command);

            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteAsync(wordId), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithNonExistentWord_ThrowsInvalidOperationException()
        {
            var wordId = Guid.NewGuid();
            var command = new DeleteSensitiveWordCommand(wordId);

            _mockRepository.Setup(r => r.GetByIdAsync(wordId)).ReturnsAsync((SensitiveWord?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WithNullCommand_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.HandleAsync(null!));
        }

        [Fact]
        public async Task HandleAsync_WhenDeleteFails_ReturnsFalse()
        {
            var wordId = Guid.NewGuid();
            var existingWord = new SensitiveWord("SELECT");
            var command = new DeleteSensitiveWordCommand(wordId);

            _mockRepository.Setup(r => r.GetByIdAsync(wordId)).ReturnsAsync(existingWord);
            _mockRepository.Setup(r => r.DeleteAsync(wordId)).ReturnsAsync(false);

            var result = await _handler.HandleAsync(command);

            Assert.False(result);
            _mockRepository.Verify(r => r.DeleteAsync(wordId), Times.Once);
        }
    }
}
