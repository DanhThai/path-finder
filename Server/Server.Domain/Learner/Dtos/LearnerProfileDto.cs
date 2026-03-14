using Common.Domain;

namespace Server.Domain.Learner
{
    public class LearnerProfileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string LearnerID { get; set; }
        public DocumentProperty Avatar { get; set; }

        public Guid? MajorId { get; set; }
        public string MajorName { get; set; }
    }
}
