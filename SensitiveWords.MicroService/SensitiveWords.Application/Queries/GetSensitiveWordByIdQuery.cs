using System;

namespace Flash.SensitiveWords.Application.Queries
{
    public class GetSensitiveWordByIdQuery
    {
        public Guid Id { get; set; }

        public GetSensitiveWordByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
