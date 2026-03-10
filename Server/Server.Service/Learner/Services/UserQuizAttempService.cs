using AutoMapper;
using Common.Repository;
using Server.Domain.Learner;

namespace Server.Service.Learner
{
    public class UserQuizAttempService(
        IDBRepository _repository,
        IMapper _mapper
        ) : IUserQuizAttempService
    {
        public async Task<UserQuizAttempDto> GetQuizResultAsync(Guid mycourseId)
        {
            var userQuiz = await _repository.GetAsync<UserQuizAttempEntity>(x => x.MyCourseId == mycourseId);
            var result = _mapper.Map<UserQuizAttempDto>(userQuiz);

            return result;
        }
    }
}
