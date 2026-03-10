using Common.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Repository
{
    public class CourseEntity : BaseEntity
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string VideoURL { get; set; }
        public CourseType CourseType { get; set; }
        public CourseStatus Status { get; set; }
        [Column(TypeName = "jsonb")]
        public DocumentProperty Image { get; set; }

        public virtual ICollection<CourseTaskEntity> Tasks { get; set; }
        public virtual ICollection<QuestionEntity> Questions { get; set; }
        public virtual ICollection<MyCourseEntity> EnrolledLearners { get; set; }
    }
}
