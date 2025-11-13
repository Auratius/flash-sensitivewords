using System;
using Xunit;
using Flash.SensitiveWords.Domain.Entities;

namespace Flash.SensitiveWords.Tests.Unit.Domain.Entities
{
    public class SensitiveWordTests
    {
        [Fact]
        public void Constructor_WithValidWord_CreatesInstance()
        {
            var word = "SELECT";

            var sensitiveWord = new SensitiveWord(word);

            Assert.NotEqual(Guid.Empty, sensitiveWord.Id);
            Assert.Equal("SELECT", sensitiveWord.Word);
            Assert.True(sensitiveWord.IsActive);
            Assert.True(sensitiveWord.CreatedAt <= DateTime.UtcNow);
            Assert.True(sensitiveWord.UpdatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Constructor_WithWhitespace_TrimsAndUppercases()
        {
            var word = "  select  ";

            var sensitiveWord = new SensitiveWord(word);

            Assert.Equal("SELECT", sensitiveWord.Word);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidWord_ThrowsArgumentException(string? invalidWord)
        {
            Assert.Throws<ArgumentException>(() => new SensitiveWord(invalidWord!));
        }

        [Fact]
        public void UpdateWord_WithValidWord_UpdatesWordAndTimestamp()
        {
            var sensitiveWord = new SensitiveWord("SELECT");
            var originalUpdatedAt = sensitiveWord.UpdatedAt;
            System.Threading.Thread.Sleep(10);

            sensitiveWord.UpdateWord("DROP");

            Assert.Equal("DROP", sensitiveWord.Word);
            Assert.True(sensitiveWord.UpdatedAt > originalUpdatedAt);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateWord_WithInvalidWord_ThrowsArgumentException(string? invalidWord)
        {
            var sensitiveWord = new SensitiveWord("SELECT");

            Assert.Throws<ArgumentException>(() => sensitiveWord.UpdateWord(invalidWord!));
        }

        [Fact]
        public void Activate_SetsIsActiveToTrue()
        {
            var sensitiveWord = new SensitiveWord("SELECT");
            sensitiveWord.Deactivate();
            var originalUpdatedAt = sensitiveWord.UpdatedAt;
            System.Threading.Thread.Sleep(10);

            sensitiveWord.Activate();

            Assert.True(sensitiveWord.IsActive);
            Assert.True(sensitiveWord.UpdatedAt > originalUpdatedAt);
        }

        [Fact]
        public void Deactivate_SetsIsActiveToFalse()
        {
            var sensitiveWord = new SensitiveWord("SELECT");
            var originalUpdatedAt = sensitiveWord.UpdatedAt;
            System.Threading.Thread.Sleep(10);

            sensitiveWord.Deactivate();

            Assert.False(sensitiveWord.IsActive);
            Assert.True(sensitiveWord.UpdatedAt > originalUpdatedAt);
        }

        [Fact]
        public void FullConstructor_WithAllParameters_CreatesInstance()
        {
            var id = Guid.NewGuid();
            var createdAt = DateTime.UtcNow.AddDays(-1);
            var updatedAt = DateTime.UtcNow;

            var sensitiveWord = new SensitiveWord(id, "SELECT", true, createdAt, updatedAt);

            Assert.Equal(id, sensitiveWord.Id);
            Assert.Equal("SELECT", sensitiveWord.Word);
            Assert.True(sensitiveWord.IsActive);
            Assert.Equal(createdAt, sensitiveWord.CreatedAt);
            Assert.Equal(updatedAt, sensitiveWord.UpdatedAt);
        }
    }
}
