using AutoMapper;
using Common.Repository;
using Server.Domain.Admin;
using Server.Domain.Learner;

namespace Server.Service.Learner
{
    public class TaskService(
        IDBRepository _repository,
        IMapper _mapper
        ) : ITaskService
    {
        public async Task<List<CourseTaskDto>> GetTaskByCourseId(Guid courseId)
        {
            var tasks = await _repository.GetAsync<CourseTaskEntity>(x => x.CourseId == courseId);
            var result = _mapper.Map<List<CourseTaskDto>>(tasks);

            return result;
        }

        public async Task<TaskResultDto> GetTaskResultByTaskId(Guid taskId)
        {
            var taskResult = await _repository.GetAsync<TaskResultEntity>(x => x.TaskId == taskId);
            var result = _mapper.Map<TaskResultDto>(taskResult);

            return result;
        }
    }
}
