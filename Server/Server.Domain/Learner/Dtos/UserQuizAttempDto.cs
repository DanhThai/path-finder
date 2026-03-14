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
        public bool IsSubmitted { get; set; }
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
        public List<SubmitQuestionDto> Questions { get; set; }
    }

    public class SubmitQuestionDto
    {
        public Guid QuestionId { get; set; }
        public List<Guid> AnswerIds { get; set; }
    }

    public class UserQuizDashBoardDto
    {
        public Guid MyCourseId { get; set; }
        public string CourseTitle { get; set; }
        public int Score { get; set; }
        public int TotalQuestion { get; set; }
        public DateTimeOffset SubmittedAt { get; set; }
    }
}
