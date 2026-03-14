using Common.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Repository
{
    public class TaskResultEntity : BaseEntity
    {
        public Guid TaskId { get; set; }
        public Guid MyCourseId { get; set; }
        public string TaskName { get; set; }
        public string TaskSummary { get; set; }
        public int Order { get; set; }
        public bool IsCompleted { get; set; }
        public DateTimeOffset SubmittedAt { get; set; }

        [Column(TypeName = "jsonb")]
        public DocumentProperty SubmitAssignment { get; set; }

        public CourseTaskEntity CourseTask { get; set; }
        public MyCourseEntity MyCourse { get; set; }
        public virtual ICollection<FeedBackEntity> FeedBacks { get; set; }
    }
}
