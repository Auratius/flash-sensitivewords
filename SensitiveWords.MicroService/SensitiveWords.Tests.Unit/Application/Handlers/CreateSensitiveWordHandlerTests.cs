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
    public class CreateSensitiveWordHandlerTests
    {
        private readonly Mock<ISensitiveWordRepository> _mockRepository;
        private readonly CreateSensitiveWordHandler _handler;

        public CreateSensitiveWordHandlerTests()
        {
            _mockRepository = new Mock<ISensitiveWordRepository>();
            _handler = new CreateSensitiveWordHandler(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CreateSensitiveWordHandler(null!));
        }

        [Fact]
        public async Task HandleAsync_WithValidCommand_CreatesWord()
        {
            var command = new CreateSensitiveWordCommand("SELECT");
            var expectedId = Guid.NewGuid();
            _mockRepository.Setup(r => r.ExistsAsync("SELECT")).ReturnsAsync(false);
            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<SensitiveWord>())).ReturnsAsync(expectedId);

            var result = await _handler.HandleAsync(command);

            Assert.Equal(expectedId, result);
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<SensitiveWord>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithExistingWord_ThrowsInvalidOperationException()
        {
            var command = new CreateSensitiveWordCommand("SELECT");
            _mockRepository.Setup(r => r.ExistsAsync("SELECT")).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<SensitiveWord>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WithNullCommand_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.HandleAsync(null!));
        }
    }
}
