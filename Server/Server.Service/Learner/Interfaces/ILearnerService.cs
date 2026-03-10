using Common.Domain;
using Common.Service;
using Server.Domain.Learner;

namespace Server.Service.Learner
{
    public interface ILearnerService : IScopeDependency
    {
        Task<TableInfo<ViewCourseDto>> GetCourseByUserIdPaging(CTableParameter parameter);
        Task<MyCourseDto> GetDetail(Guid id);
        Task<bool> SubmitQuiz(SubmitQuizDto dto);
        Task<bool> SubmitTask(TaskResultDto dto);
        Task<bool> Delete(Guid id);
    }
}
