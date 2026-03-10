namespace Common.Repository
{
    public class FeedBackEntity : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public Guid LearnerTaskId { get; set; }

        public TaskResultEntity LearnerTask { get; set; }
    }
}
