namespace Common.Repository
{
    public class DocumentEntity : BaseEntity
    {
        public string Url { get; set; }
        public Guid CourseId { get; set; }
    }
}
