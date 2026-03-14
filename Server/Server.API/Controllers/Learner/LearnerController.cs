using Common.Domain;
using Microsoft.AspNetCore.Mvc;
using Server.API;
using Server.Domain.Learner;
using Server.Service.Learner;

namespace Server.Learner.API
{
    [ApiVersion("1")]
    public class LearnerController : LearnerAPIBaseController
    {
        private readonly ILearnerService _learnerService;
        public LearnerController(IHttpContextAccessor accessor, ILearnerService learnerService) : base(accessor)
        {
            _learnerService = learnerService;
        }

        [HttpPost("mycourse/paging")]
        public async Task<TableInfo<ViewCourseDto>> GetCourseByUserIdPaging([FromBody] CTableParameter parameter)
        {
            return await _learnerService.GetCourseByUserIdPaging(parameter);
        }

        [HttpGet("mycourse/{id}")]
        public async Task<MyCourseDto> GetDetail(Guid id)
        {
            return await _learnerService.GetDetail(id);
        }

        [HttpPost("mycourse/submitquiz")]
        public async Task<bool> SubmitQuiz([FromBody] SubmitQuizDto dto)
        {
            return await _learnerService.SubmitQuiz(dto);
        }

        [HttpPost("mycourse/submittask")]
        public async Task<bool> SubmitTask([FromBody] TaskResultDto dto)
        {
            return await _learnerService.SubmitTask(dto);
        }

        [HttpDelete("mycourse/{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _learnerService.Delete(id);
        }
    }
}
