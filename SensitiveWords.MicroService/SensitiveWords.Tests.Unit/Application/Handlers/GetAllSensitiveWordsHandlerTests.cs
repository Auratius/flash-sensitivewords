using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Flash.SensitiveWords.Application.Handlers;
using Flash.SensitiveWords.Application.Queries;
using Flash.SensitiveWords.Domain.Entities;
using Flash.SensitiveWords.Domain.Interfaces;

namespace Flash.SensitiveWords.Tests.Unit.Application.Handlers
{
    public class GetAllSensitiveWordsHandlerTests
    {
        private readonly Mock<ISensitiveWordRepository> _mockRepository;
        private readonly GetAllSensitiveWordsHandler _handler;

        public GetAllSensitiveWordsHandlerTests()
        {
            _mockRepository = new Mock<ISensitiveWordRepository>();
            _handler = new GetAllSensitiveWordsHandler(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GetAllSensitiveWordsHandler(null!));
        }

        [Fact]
        public async Task HandleAsync_WithValidQuery_ReturnsAllWords()
        {
            var words = new List<SensitiveWord>
            {
                new SensitiveWord("SELECT"),
                new SensitiveWord("DROP"),
                new SensitiveWord("DELETE")
            };
            var query = new GetAllSensitiveWordsQuery();

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(words);

            var result = await _handler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Contains(result, w => w.Word == "SELECT");
            Assert.Contains(result, w => w.Word == "DROP");
            Assert.Contains(result, w => w.Word == "DELETE");
        }

        [Fact]
        public async Task HandleAsync_WithEmptyRepository_ReturnsEmptyList()
        {
            var query = new GetAllSensitiveWordsQuery();

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<SensitiveWord>());

            var result = await _handler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task HandleAsync_WithNullQuery_ReturnsAllWords()
        {
            // The handler accepts null query and treats it as getting all words
            var words = new List<SensitiveWord>
            {
                new SensitiveWord("SELECT"),
                new SensitiveWord("DROP")
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(words);

            var result = await _handler.HandleAsync(null!);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task HandleAsync_WithMixedActiveStatus_ReturnsCorrectDtos()
        {
            var activeWord = new SensitiveWord(Guid.NewGuid(), "SELECT", true, DateTime.UtcNow, DateTime.UtcNow);
            var inactiveWord = new SensitiveWord(Guid.NewGuid(), "DROP", false, DateTime.UtcNow, DateTime.UtcNow);
            var words = new List<SensitiveWord> { activeWord, inactiveWord };
            var query = new GetAllSensitiveWordsQuery();

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(words);

            var result = await _handler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var activeDto = result.First(w => w.Word == "SELECT");
            Assert.True(activeDto.IsActive);

            var inactiveDto = result.First(w => w.Word == "DROP");
            Assert.False(inactiveDto.IsActive);
        }
    }
}
