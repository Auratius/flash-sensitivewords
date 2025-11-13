using System;

namespace Flash.SensitiveWords.Domain.Entities
{
    public class SensitiveWord
    {
        public Guid Id { get; private set; }
        public string Word { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private SensitiveWord() { }

        public SensitiveWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                throw new ArgumentException("Word cannot be null or empty", nameof(word));

            Id = Guid.NewGuid();
            Word = word.Trim().ToUpperInvariant();
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public SensitiveWord(Guid id, string word, bool isActive, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            Word = word;
            IsActive = isActive;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public void UpdateWord(string newWord)
        {
            if (string.IsNullOrWhiteSpace(newWord))
                throw new ArgumentException("Word cannot be null or empty", nameof(newWord));

            Word = newWord.Trim().ToUpperInvariant();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
