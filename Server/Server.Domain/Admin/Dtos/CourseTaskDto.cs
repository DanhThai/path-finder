using Common.Domain;

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
}
