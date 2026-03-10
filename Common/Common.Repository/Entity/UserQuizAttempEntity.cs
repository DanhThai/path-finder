using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Repository
{
    public class UserQuizAttempEntity : BaseEntity
    {
        public Guid MyCourseId { get; set; }
        public int Score { get; set; }
        public int TotalQuestion { get; set; }
        public int Progress { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }


        [Column(TypeName = "jsonb")]
        public List<QuestionEntity> Questions { get; set; }
        public MyCourseEntity MyCourse { get; set; }
    }
}
