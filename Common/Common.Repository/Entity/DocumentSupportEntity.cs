namespace Common.Repository
{
    public class DocumentSupportEntity : BaseEntity
    {
        public string Url { get; set; }
        public Guid TaskId { get; set; }
    }
}
