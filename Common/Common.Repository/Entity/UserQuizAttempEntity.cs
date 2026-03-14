using Common.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Repository
{
    public class UserQuizAttempEntity : BaseEntity
    {
        public Guid MyCourseId { get; set; }
        public int Score { get; set; }
        public int TotalQuestion { get; set; }
        public bool IsSubmitted { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }


        [Column(TypeName = "jsonb")]
        public List<QuestionProperty> Questions { get; set; }
        public MyCourseEntity MyCourse { get; set; }
    }

    public class QuestionProperty
    {
        public Guid Id { get; set; }
        public int QuestionOrder { get; set; }
        public string Name { get; set; }
        public CQuestionType QuestionType { get; set; }
        public bool IsCorrect { get; set; }
        public List<AnswerProperty> Answers { get; set; }
    }
}
