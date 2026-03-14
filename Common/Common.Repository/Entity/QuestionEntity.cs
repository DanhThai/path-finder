using Common.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Repository
{
    public class QuestionEntity : BaseEntity
    {
        public Guid CourseId { get; set; }
        public int QuestionOrder { get; set; }
        public string Name { get; set; }
        public CQuestionType QuestionType { get; set; }

        [Column(TypeName = "jsonb")]
        public List<AnswerProperty> Answers { get; set; }
        public CourseEntity Course { get; set; }
    }

    public class AnswerProperty
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string ExplainDescription { get; set; }
        public bool IsCorrect { get; set; }
        public bool? IsSelected { get; set; }
    }
}
