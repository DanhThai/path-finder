using Common.Domain;

namespace Server.Domain.Admin
{
    public class QuestionDto : BaseDto
    {
        public Guid CourseId { get; set; }
        public int QuestionOrder { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsCorrect { get; set; }
        public CQuestionType QuestionType { get; set; }
        public List<AnswerPropertyDto> Answers { get; set; }
    }

    public class AnswerPropertyDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string ExplainDescription { get; set; }
        public bool IsCorrect { get; set; }
        public bool? IsSelected { get; set; }
    }
}
