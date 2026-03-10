using Common.Domain;

namespace Server.Domain.Learner
{
    public class FeedbackDto : BaseDto
    {
        public string Content { get; set; }
        public Guid UserId { get; set; }
    }
}
