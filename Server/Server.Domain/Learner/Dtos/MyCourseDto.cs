using Common.Domain;
using Server.Domain.Admin;

namespace Server.Domain.Learner
{
    public class MyCourseDto : BaseDto
    {
        public Guid UserId { get; set; }
        public ProgressStatusEnum ProgressStatusEnum { get; set; }
        public ApplyStatus ApplyStatus { get; set; }
        public CourseDetailDto Course { get; set; }

        public bool IsSubmittedQuiz { get; set; }
        public string QuizResult { get; set; }

        public List<TaskResultDto> TaskResults { get; set; }
        public int TaskProgress { get; set; }
    }

    public class MyCourseCreateDto : BaseDto
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
    }

    public class ViewCourseDto : BaseDto
    {
        public Guid UserId { get; set; }
        public ProgressStatusEnum ProgressStatusEnum { get; set; }
        public ApplyStatus ApplyStatus { get; set; }
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int Duration { get; set; }
        public string CourseSummary { get; set; }
        public string VideoURL { get; set; }
        public CourseStatus CourseStatus { get; set; }
        public CourseType CourseType { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TaskCount { get; set; }
        public int EnrolledCount { get; set; }
        public bool HasApplied { get; set; }
        public DocumentProperty Image { get; set; }
    }
}
