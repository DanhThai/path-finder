using Common.Service;
using Server.Domain.Learner;

namespace Server.Service.Learner
{
    public interface IUserQuizAttempService : IScopeDependency
    {
        Task<UserQuizAttempDto> GetQuizResultAsync(Guid mycourseId);
    }
}
