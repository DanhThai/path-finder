using Common.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Repository
{
    public class CourseTaskEntity : BaseEntity
    {
        public Guid CourseId { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Introduce { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int Order { get; set; }

        [Column(TypeName = "jsonb")]
        public List<DocumentProperty> SupportingDocuments { get; set; }

        [Column(TypeName = "jsonb")]
        public List<DocumentProperty> ExampleDocuments { get; set; }

        public CourseEntity Course { get; set; }
    }
}