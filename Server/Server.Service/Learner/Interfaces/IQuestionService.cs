using Common.Service;
using Server.Domain.Admin;

namespace Server.Service.Learner
{
    public interface IQuestionService : IScopeDependency
    {
        Task<List<QuestionDto>> GetQuestionByCourseId(Guid courseId);
    }
}
