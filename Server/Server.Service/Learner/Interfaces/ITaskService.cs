using Common.Service;
using Server.Domain.Admin;
using Server.Domain.Learner;

namespace Server.Service.Learner
{
    public interface ITaskService : IScopeDependency
    {
        Task<List<CourseTaskViewDetailDto>> GetTaskByCourseId(Guid courseId);
        Task<CourseTaskViewDetailDto> GetLearnerTask(Guid learnerTaskId);
        Task<TaskResultDto> GetTaskResultByTaskId(Guid taskId);
    }
}
