using AutoMapper;
using Common.Domain;
using Common.Repository;
using Microsoft.EntityFrameworkCore;
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
            var userQuiz = await _repository.GetSet<UserQuizAttempEntity>(x => x.MyCourseId == mycourseId)
                .FirstOrDefaultAsync() ?? throw new NotExistException("Quiz");
            var result = _mapper.Map<UserQuizAttempDto>(userQuiz);

            return result;
        }
    }
}
