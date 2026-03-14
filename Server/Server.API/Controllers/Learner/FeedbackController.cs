using Common.Domain;
using Microsoft.AspNetCore.Mvc;
using Server.API;
using Server.Domain.Learner;
using Server.Service.Learner;

namespace Server.Learner.API
{
    [ApiVersion("1")]
    public class FeedbackController : LearnerAPIBaseController
    {
        private readonly IFeedbackService _feedbackService;
        public FeedbackController(IHttpContextAccessor accessor, IFeedbackService feedbackService) : base(accessor)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost("feedback/paging")]
        public async Task<TableInfo<LearnerFeedbackDto>> GetPaging([FromBody] CTableParameter parameter)
        {
            return await _feedbackService.GetPaging(parameter);
        }
    }
}
