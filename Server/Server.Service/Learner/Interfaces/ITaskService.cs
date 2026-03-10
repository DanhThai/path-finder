using Common.Service;
using Server.Domain.Admin;
using Server.Domain.Learner;

namespace Server.Service.Learner
{
    public interface ITaskService : IScopeDependency
    {
        Task<List<CourseTaskDto>> GetTaskByCourseId(Guid courseId);
        Task<TaskResultDto> GetTaskResultByTaskId(Guid taskId);
    }
}
