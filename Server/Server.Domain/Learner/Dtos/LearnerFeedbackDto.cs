using Common.Domain;

namespace Server.Domain.Learner
{
    public class LearnerFeedbackDto : BaseDto
    {
        public Guid learnerTaskId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset SendOn { get; set; }
        public string SendBy { get; set; }
        public Guid SendById { get; set; }
    }
}
