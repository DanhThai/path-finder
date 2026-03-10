using Common.Domain;
using Microsoft.AspNetCore.Mvc;
using Server.API;
using Server.Domain.Admin;
using Server.Service.Admin;

namespace Server.Admin.API
{
    [ApiVersion("1")]
    public class EnrollmentLearnerController : APIBaseController
    {
        private readonly IEnrollmentLearnerService _enrollmentLearnerService;

        public EnrollmentLearnerController(IHttpContextAccessor accessor, IEnrollmentLearnerService enrollmentLearnerService) : base(accessor)
        {
            _enrollmentLearnerService = enrollmentLearnerService;
        }

        [HttpPost("enrollmentlearner/paging/{courseId}")]
        public async Task<TableInfo<EnrollmentLearnerDto>> GetPaging(Guid courseId, [FromBody] CTableParameter parameter)
        {
            return await _enrollmentLearnerService.GetPaging(parameter, courseId);
        }

        [HttpGet("enrollmentlearner/{myCourseId}")]
        public async Task<LearnerEnrollmentResultDto> GetDetail(Guid myCourseId)
        {
            return await _enrollmentLearnerService.GetEnrollmentResult(myCourseId);
        }

        [HttpPost("enrollmentlearner/feedback")]
        public async Task<bool> GetPaging([FromBody] FeedBackDto dto)
        {
            return await _enrollmentLearnerService.SendFeedback(dto);
        }
    }
}
