using Common.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.API;
using Server.Domain.Learner;
using Server.Service.Learner;

namespace Server.Learner.API
{
    [ApiVersion("1")]
    public class LearnerProfileController : LearnerAPIBaseController
    {
        private readonly ILearnerProfileService _learnerProfileService;
        public LearnerProfileController(IHttpContextAccessor accessor, ILearnerProfileService learnerProfileService) : base(accessor)
        {
            _learnerProfileService = learnerProfileService;
        }

        [HttpGet("myprofile")]
        public async Task<LearnerProfileDto> GetMyProfile()
        {
            return await _learnerProfileService.GetMyProfile();
        }

        [HttpGet("myprofile/dashboard")]
        public async Task<ProfileDashBoardDto> GetMyProfileDashBoard()
        {
            return await _learnerProfileService.GetMyProfileDashBoard();
        }

        [HttpPut("myprofile")]
        public async Task<bool> UpdateLearnerProfile([FromBody] LearnerProfileDto dto)
        {
            return await _learnerProfileService.UpdateLearnerProfile(dto);
        }
    }
}
