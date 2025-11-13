namespace Flash.SensitiveWords.Application.Queries
{
    public class SanitizeMessageQuery
    {
        public string Message { get; set; }

        public SanitizeMessageQuery(string message)
        {
            Message = message;
        }
    }
}
