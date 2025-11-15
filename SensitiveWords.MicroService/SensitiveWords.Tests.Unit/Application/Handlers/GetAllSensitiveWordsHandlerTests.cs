using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly Mock<IOperationStatsRepository> _mockStatsRepo;
        private readonly GetAllSensitiveWordsHandler _handler;

        public GetAllSensitiveWordsHandlerTests()
        {
            _mockRepository = new Mock<ISensitiveWordRepository>();
            _mockStatsRepo = new Mock<IOperationStatsRepository>();
            _handler = new GetAllSensitiveWordsHandler(_mockRepository.Object, _mockStatsRepo.Object);
        }

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GetAllSensitiveWordsHandler(null!, _mockStatsRepo.Object));
        }

        [Fact]
        public void Constructor_WithNullStatsRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GetAllSensitiveWordsHandler(_mockRepository.Object, null!));
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
            _mockStatsRepo
                .Setup(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Contains("SELECT", result.Select(r => r.Word));
            Assert.Contains("DROP", result.Select(r => r.Word));
            Assert.Contains("DELETE", result.Select(r => r.Word));

            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockStatsRepo.Verify(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithEmptyRepository_ReturnsEmptyList()
        {
            var query = new GetAllSensitiveWordsQuery();

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<SensitiveWord>());
            _mockStatsRepo
                .Setup(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Empty(result);

            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockStatsRepo.Verify(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithNullQuery_ReturnsAllWords()
        {
            // Handler treats null query as "get all"
            var words = new List<SensitiveWord>
            {
                new SensitiveWord("SELECT"),
                new SensitiveWord("DROP")
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(words);
            _mockStatsRepo
                .Setup(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.HandleAsync(null!);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockStatsRepo.Verify(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithMixedActiveStatus_ReturnsCorrectDtos()
        {
            var activeWord = new SensitiveWord(Guid.NewGuid(), "SELECT", true, DateTime.UtcNow, DateTime.UtcNow);
            var inactiveWord = new SensitiveWord(Guid.NewGuid(), "DROP", false, DateTime.UtcNow, DateTime.UtcNow);
            var words = new List<SensitiveWord> { activeWord, inactiveWord };
            var query = new GetAllSensitiveWordsQuery();

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(words);
            _mockStatsRepo
                .Setup(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var activeDto = result.First(w => w.Word == "SELECT");
            Assert.True(activeDto.IsActive);

            var inactiveDto = result.First(w => w.Word == "DROP");
            Assert.False(inactiveDto.IsActive);

            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockStatsRepo.Verify(r => r.IncrementAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
