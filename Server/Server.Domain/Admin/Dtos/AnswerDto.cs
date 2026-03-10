using Common.Domain;

namespace Server.Domain.Admin
{
    public class AnswerDto : BaseDto
    {
        public int AnswerOrder { get; set; }
        public string Name { get; set; }
        public string ExplainDescription { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class AnswerIdAndNameDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
