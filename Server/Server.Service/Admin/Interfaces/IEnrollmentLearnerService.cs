using Common.Domain;
using Common.Service;
using Server.Domain.Admin;

namespace Server.Service.Admin
{
    public interface IEnrollmentLearnerService : IScopeDependency
    {
        Task<TableInfo<EnrollmentLearnerDto>> GetPaging(CTableParameter parameter, Guid courseId);
        Task<LearnerEnrollmentResultDto> GetEnrollmentResult(Guid myCourseId);
        Task<bool> SendFeedback(FeedBackDto feedBackDto);
    }
}
