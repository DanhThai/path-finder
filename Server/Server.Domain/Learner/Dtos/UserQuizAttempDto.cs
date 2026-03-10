using Common.Domain;
using Server.Domain.Admin;

namespace Server.Domain.Learner
{
    public class UserQuizAttempDto : BaseDto
    {
        public Guid MyCourseId { get; set; }
        public int Score { get; set; }
        public int TotalQuestion { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public int Progress { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }

    public class SubmitQuizDto : BaseDto
    {
        public Guid MyCourseId { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public int Progress { get; set; }
        public int Score { get; set; }
        public int TotalQuestion { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }
}
