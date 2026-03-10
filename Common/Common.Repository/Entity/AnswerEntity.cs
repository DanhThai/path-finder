namespace Common.Repository
{
    public class AnswerEntity : BaseEntity
    {
        public int AnswerOrder { get; set; }
        public string Name { get; set; }
        public string ExplainDescription { get; set; }
        public bool IsCorrect { get; set; }
        public Guid QuestionId { get; set; }

        public QuestionEntity Question { get; set; }
    }
}
