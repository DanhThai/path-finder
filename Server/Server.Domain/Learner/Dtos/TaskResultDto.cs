using Common.Domain;

namespace Server.Domain.Learner
{
    public class TaskResultDto : BaseDto
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskSummary { get; set; }
        public bool IsCompleted { get; set; }
        public Guid MyCourseId { get; set; }
        public DateTimeOffset SubmittedAt { get; set; }
        public DocumentProperty SubmitAssignment { get; set; }
    }

    public class CourseTaskDashBoardDto
    {
        public Guid MyCourseId { get; set; }
        public string CourseTitle { get; set; }
        public int CompletedPercent { get; set; }
        public DateTimeOffset SubmittedAt { get; set; }
    }
}
