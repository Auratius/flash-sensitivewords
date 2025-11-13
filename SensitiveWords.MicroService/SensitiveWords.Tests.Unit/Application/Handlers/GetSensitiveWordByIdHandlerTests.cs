using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Flash.SensitiveWords.Application.Handlers;
using Flash.SensitiveWords.Application.Queries;
using Flash.SensitiveWords.Domain.Entities;
using Flash.SensitiveWords.Domain.Interfaces;

namespace Flash.SensitiveWords.Tests.Unit.Application.Handlers
{
    public class GetSensitiveWordByIdHandlerTests
    {
        private readonly Mock<ISensitiveWordRepository> _mockRepository;
        private readonly GetSensitiveWordByIdHandler _handler;

        public GetSensitiveWordByIdHandlerTests()
        {
            _mockRepository = new Mock<ISensitiveWordRepository>();
            _handler = new GetSensitiveWordByIdHandler(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GetSensitiveWordByIdHandler(null!));
        }

        [Fact]
        public async Task HandleAsync_WithValidQuery_ReturnsDto()
        {
            var wordId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow.AddDays(-1);
            var updatedAt = DateTime.UtcNow;
            var word = new SensitiveWord(wordId, "SELECT", true, createdAt, updatedAt);
            var query = new GetSensitiveWordByIdQuery(wordId);

            _mockRepository.Setup(r => r.GetByIdAsync(wordId)).ReturnsAsync(word);

            var result = await _handler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Equal(wordId, result.Id);
            Assert.Equal("SELECT", result.Word);
            Assert.True(result.IsActive);
            Assert.Equal(createdAt, result.CreatedAt);
            Assert.Equal(updatedAt, result.UpdatedAt);
        }

        [Fact]
        public async Task HandleAsync_WithNonExistentWord_ReturnsNull()
        {
            var wordId = Guid.NewGuid();
            var query = new GetSensitiveWordByIdQuery(wordId);

            _mockRepository.Setup(r => r.GetByIdAsync(wordId)).ReturnsAsync((SensitiveWord?)null);

            var result = await _handler.HandleAsync(query);

            Assert.Null(result);
        }

        [Fact]
        public async Task HandleAsync_WithNullQuery_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.HandleAsync(null!));
        }

        [Fact]
        public async Task HandleAsync_WithInactiveWord_ReturnsCorrectStatus()
        {
            var wordId = Guid.NewGuid();
            var word = new SensitiveWord(wordId, "DROP", false, DateTime.UtcNow, DateTime.UtcNow);
            var query = new GetSensitiveWordByIdQuery(wordId);

            _mockRepository.Setup(r => r.GetByIdAsync(wordId)).ReturnsAsync(word);

            var result = await _handler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.False(result.IsActive);
        }
    }
}
