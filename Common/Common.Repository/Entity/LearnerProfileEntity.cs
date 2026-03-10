namespace Common.Repository
{
    public class LearnerProfileEntity : BaseEntity
    {
        public string LearnerID { get; set; }
        public Guid MajorId { get; set; }

        public virtual AccountEntity Account { get; set; }
        //public virtual LearnerMajorEntity Major { get; set; }
    }
}
