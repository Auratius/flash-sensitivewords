namespace Flash.SensitiveWords.Application.DTOs
{
    public class SanitizeResponse
    {
        public string OriginalMessage { get; set; }
        public string SanitizedMessage { get; set; }
        public int WordsReplaced { get; set; }
    }
}
