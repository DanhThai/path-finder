using Common.Service;
using Server.Domain.Learner;

namespace Server.Service.Learner
{
    public interface ILearnerProfileService : IScopeDependency
    {
        Task<LearnerProfileDto> GetMyProfile();
        Task<ProfileDashBoardDto> GetMyProfileDashBoard();
        Task<bool> UpdateLearnerProfile(LearnerProfileDto dto);
    }
}
