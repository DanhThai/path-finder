namespace Server.Domain.Learner
{
    public class ProfileDashBoardDto
    {
        public int CountMyCourse { get; set; }
        public int CountTaskSubmitted { get; set; }
        public int CountQuizSubmitted { get; set; }
        public int CountFeedback { get; set; }
        public List<LearnerFeedbackDto> Feedbacks { get; set; }
        public List<UserQuizDashBoardDto> Quizzes { get; set; }
        public List<CourseTaskDashBoardDto> Tasks { get; set; }
    }
}
