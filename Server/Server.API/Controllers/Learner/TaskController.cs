using Microsoft.AspNetCore.Mvc;
using Server.API;
using Server.Domain.Admin;
using Server.Domain.Learner;
using Server.Service.Learner;

namespace Server.Learner.API
{
    [ApiVersion("1")]
    public class TaskController : APIBaseController
    {
        private readonly ITaskService _taskService;
        public TaskController(IHttpContextAccessor accessor, ITaskService taskService) : base(accessor)
        {
            _taskService = taskService;
        }

        [HttpGet("task/courseid={courseId}")]
        public async Task<List<CourseTaskDto>> GetTaskByCourseId([FromRoute] Guid courseId)
        {
            return await _taskService.GetTaskByCourseId(courseId);
        }

        [HttpGet("taskresult/taskid={taskId}")]
        public async Task<TaskResultDto> GetTaskResultByTaskId([FromRoute] Guid taskId)
        {
            return await _taskService.GetTaskResultByTaskId(taskId);
        }
    }
}
