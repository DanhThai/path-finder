using Common.Domain;

namespace Common.Repository
{
    public class MyCourseEntity : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public ProgressStatusEnum ProgressStatusEnum { get; set; }
        public ApplyStatus ApplyStatus { get; set; }

        public LearnerProfileEntity Learner { get; set; }
        public CourseEntity Course { get; set; }
        public virtual ICollection<UserQuizAttempEntity> UserQuizAttemps { get; set; }
        public virtual ICollection<TaskResultEntity> TaskResults { get; set; }
    }
}
