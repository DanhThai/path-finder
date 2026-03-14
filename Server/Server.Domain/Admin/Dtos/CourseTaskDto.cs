using Common.Domain;
using Server.Domain.Learner;

namespace Server.Domain.Admin
{
    public class CourseTaskDto : BaseDto
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Introduce { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int Order { get; set; }

        public List<DocumentProperty> SupportingDocuments { get; set; }
        public List<DocumentProperty> ExampleDocuments { get; set; }
    }

    public class CourseTaskViewDetailDto : CourseTaskDto
    {
        public Guid MyCourseId { get; set; }
        public ProgressStatusEnum ProgressStatusEnum { get; set; }
        public DateTimeOffset SubmittedAt { get; set; }
        public DocumentProperty SubmitAssignment { get; set; }
        public List<LearnerFeedbackDto> Feedbacks { get; set; }
    }
}
