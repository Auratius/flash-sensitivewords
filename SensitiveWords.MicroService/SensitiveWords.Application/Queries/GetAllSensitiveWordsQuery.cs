namespace Flash.SensitiveWords.Application.Queries
{
    public class GetAllSensitiveWordsQuery
    {
        public bool? ActiveOnly { get; set; }

        public GetAllSensitiveWordsQuery(bool? activeOnly = null)
        {
            ActiveOnly = activeOnly;
        }
    }
}
