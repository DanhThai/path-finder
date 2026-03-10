using Common.Domain;

namespace Common.Repository
{
    public class StorageDocumentEntity : BaseEntity
    {
        public Guid ResourceId { get; set; }
        public string FileName { get; set; }
        public string PublicID { get; set; }
        public string URL { get; set; }
        public StorageCategory Category { get; set; }
    }
}
