using Microsoft.AspNetCore.Mvc;
using Server.API;
using Server.Domain.Learner;
using Server.Service.Learner;

namespace Server.Learner.API
{
    [ApiVersion("1")]
    public class UserAttempQuizController : LearnerAPIBaseController
    {
        private readonly IUserQuizAttempService _userQuizAttempService;
        public UserAttempQuizController(IHttpContextAccessor accessor, IUserQuizAttempService userQuizAttempService) : base(accessor)
        {
            _userQuizAttempService = userQuizAttempService;
        }

        [HttpGet("quizresult/mycourse={mycourseId}")]
        public async Task<UserQuizAttempDto> GetQuizResultAsync([FromRoute] Guid mycourseId)
        {
            return await _userQuizAttempService.GetQuizResultAsync(mycourseId);
        }
    }
}
