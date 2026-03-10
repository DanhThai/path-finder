using Common.Domain;

namespace Server.Domain.Learner
{
    public class TaskResultDto : BaseDto
    {
        public Guid TaskId { get; set; }
        public Guid MyCourseId { get; set; }
        public DateTimeOffset SubmittedAt { get; set; }
        public DocumentProperty SubmitAssignment { get; set; }
    }
}
