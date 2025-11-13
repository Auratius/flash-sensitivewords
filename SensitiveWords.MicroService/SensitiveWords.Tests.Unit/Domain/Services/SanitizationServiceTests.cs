using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Flash.SensitiveWords.Domain.Entities;
using Flash.SensitiveWords.Domain.Interfaces;
using Flash.SensitiveWords.Domain.Services;

namespace Flash.SensitiveWords.Tests.Unit.Domain.Services
{
    public class SanitizationServiceTests
    {
        private readonly Mock<ISensitiveWordRepository> _mockRepository;
        private readonly SanitizationService _service;

        public SanitizationServiceTests()
        {
            _mockRepository = new Mock<ISensitiveWordRepository>();
            _service = new SanitizationService(_mockRepository.Object);
        }

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SanitizationService(null!));
        }

        [Fact]
        public async Task SanitizeMessageAsync_WithSensitiveWords_ReplacesWords()
        {
            var words = new List<SensitiveWord>
            {
                new SensitiveWord("SELECT"),
                new SensitiveWord("DROP")
            };
            _mockRepository.Setup(r => r.GetActiveWordsAsync()).ReturnsAsync(words);

            var result = await _service.SanitizeMessageAsync("SELECT * FROM table");

            Assert.Equal("SELECT * FROM table", result.OriginalMessage);
            Assert.Equal("****** * FROM table", result.SanitizedText);
            Assert.Equal(1, result.WordsReplaced);
        }

        [Fact]
        public async Task SanitizeMessageAsync_WithNullMessage_ReturnsEmptyResult()
        {
            var result = await _service.SanitizeMessageAsync(null!);

            Assert.Empty(result.OriginalMessage);
            Assert.Empty(result.SanitizedText);
            Assert.Equal(0, result.WordsReplaced);
        }

        [Fact]
        public async Task SanitizeMessageAsync_WithEmptyMessage_ReturnsEmptyResult()
        {
            var result = await _service.SanitizeMessageAsync("");

            Assert.Empty(result.OriginalMessage);
            Assert.Empty(result.SanitizedText);
            Assert.Equal(0, result.WordsReplaced);
        }

        [Fact]
        public void SanitizeMessage_WithMultipleSensitiveWords_ReplacesAll()
        {
            var words = new List<string> { "SELECT", "FROM", "DROP" };

            var result = _service.SanitizeMessage("SELECT * FROM users DROP table", words);

            Assert.Equal("****** * **** users **** table", result.SanitizedText);
            Assert.Equal(3, result.WordsReplaced);
        }

        [Fact]
        public void SanitizeMessage_WithCaseInsensitiveMatch_ReplacesWords()
        {
            var words = new List<string> { "SELECT" };

            var result = _service.SanitizeMessage("select SELECT SeLeCt", words);

            Assert.Equal("****** ****** ******", result.SanitizedText);
            Assert.Equal(3, result.WordsReplaced);
        }

        [Fact]
        public void SanitizeMessage_WithPartialMatch_DoesNotReplace()
        {
            var words = new List<string> { "SELECT" };

            var result = _service.SanitizeMessage("SELECTED SELECTING", words);

            Assert.Equal("SELECTED SELECTING", result.SanitizedText);
            Assert.Equal(0, result.WordsReplaced);
        }

        [Fact]
        public void SanitizeMessage_WithNoSensitiveWords_ReturnsOriginal()
        {
            var words = new List<string> { "SELECT", "DROP" };

            var result = _service.SanitizeMessage("Hello World", words);

            Assert.Equal("Hello World", result.SanitizedText);
            Assert.Equal(0, result.WordsReplaced);
        }

        [Fact]
        public void SanitizeMessage_WithEmptyWordList_ReturnsOriginal()
        {
            var result = _service.SanitizeMessage("SELECT * FROM table", new List<string>());

            Assert.Equal("SELECT * FROM table", result.SanitizedText);
            Assert.Equal(0, result.WordsReplaced);
        }

        [Fact]
        public void SanitizeMessage_WithNullWordList_ReturnsOriginal()
        {
            var result = _service.SanitizeMessage("SELECT * FROM table", null!);

            Assert.Equal("SELECT * FROM table", result.SanitizedText);
            Assert.Equal(0, result.WordsReplaced);
        }

        [Fact]
        public void SanitizeMessage_WithMultiWordPhrase_Replaces()
        {
            var words = new List<string> { "SELECT * FROM" };

            var result = _service.SanitizeMessage("SELECT * FROM users", words);

            Assert.Equal("************* users", result.SanitizedText);
            Assert.Equal(1, result.WordsReplaced);
        }

        [Fact]
        public void SanitizeMessage_WithDifferentLengthWords_PreservesLength()
        {
            var words = new List<string> { "A", "SELECT", "TRANSACTION" };

            var result = _service.SanitizeMessage("A SELECT TRANSACTION", words);

            Assert.Equal("* ****** ***********", result.SanitizedText);
            Assert.Equal(3, result.WordsReplaced);
        }
    }
}
