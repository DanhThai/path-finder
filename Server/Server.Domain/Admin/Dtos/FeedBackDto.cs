namespace Server.Domain.Admin
{
    public class FeedBackDto
    {
        public Guid TaskId { get; set; }
        public Guid MyCourseId { get; set; }
        public string Content { get; set; }
    }
}
