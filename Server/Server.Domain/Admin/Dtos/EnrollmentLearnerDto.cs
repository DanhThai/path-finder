using Common.Domain;

namespace Server.Domain.Admin
{
    public class EnrollmentLearnerDto: BaseDto
    {
        public Guid LearnerId { get; set; }
        public string LearnerName { get; set; }
        public string LearnerID { get; set; }
        public ProgressStatusEnum ProgressStatus { get; set; }
        public DateTimeOffset EnrolledTime { get; set; }
    }

    public class LearnerEnrollmentResultDto
    {
        public Guid CourseId { get; set; }
        public string LearnerID { get; set; }
        public CourseType CourseType { get; set; }
        public List<LearnerTaskDto> LearnerTasks { get; set; }
        public string QuizResult { get; set; }
        public List<LearnerQuestionResultDto> QuestionResults { get; set; }
    }

    public class LearnerQuestionResultDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionName { get; set; }
        public int QuestionOrder { get; set; }
        public CQuestionType QuestionType { get; set; }
        public List<LearnerAnswerDto> Answers { get; set; }
    }

    public class LearnerTaskDto
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
        public CourseType CourseType { get; set; }
        public DocumentProperty SubmitedAssignment { get; set; }
        public DateTimeOffset SubmittedAt { get; set; }
        public List<LearnerFeedBackDto> FeedBacks { get; set; }
    }

    public class LearnerFeedBackDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTimeOffset SendOn { get; set; }
        public string SendBy { get; set; }
    }

    public class LearnerAnswerDto
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public string ExplainDescription { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsSelected { get; set; }
    }
}
