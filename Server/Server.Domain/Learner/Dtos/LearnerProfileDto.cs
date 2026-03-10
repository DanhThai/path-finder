namespace Server.Domain.Learner
{
    public class LearnerProfileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string LearnerID { get; set; }

        public Guid MajorId { get; set; }
        public string MajorName { get; set; }
        public int CountMyCourse { get; set; }
        public int CountTaskSubmitted { get; set; }
        public int CountQuizSubmitted { get; set; }
        public int CountFeedback { get; set; }
    }
}
