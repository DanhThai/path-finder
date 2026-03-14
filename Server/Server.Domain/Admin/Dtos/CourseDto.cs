using Common.Domain;

namespace Server.Domain.Admin
{
    public class CourseDto : BaseDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public int Duration { get; set; }
        public CourseType CourseType { get; set; }
        public CourseStatus Status { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TaskCount { get; set; }
        public int QuizCount { get; set; }
    }

    public class CourseDetailDto : BaseDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public int Duration { get; set; }
        public string VideoURL { get; set; }
        public CourseType CourseType { get; set; }
        public CourseStatus Status { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DocumentProperty Image { get; set; }

        public List<CourseTaskDto> Tasks { get; set; }
        public List<QuestionDto> Questions { get; set; }
        public int TaskCount { get; set; }
        public int QuestionCount { get; set; }

        public bool HasApplied { get; set; }
        public Guid MyCourseId { get; set; }
    }
}
