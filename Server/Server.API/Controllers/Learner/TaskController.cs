using Microsoft.AspNetCore.Mvc;
using Server.API;
using Server.Domain.Admin;
using Server.Domain.Learner;
using Server.Service.Learner;

namespace Server.Learner.API
{
    [ApiVersion("1")]
    public class TaskController : LearnerAPIBaseController
    {
        private readonly ITaskService _taskService;
        public TaskController(IHttpContextAccessor accessor, ITaskService taskService) : base(accessor)
        {
            _taskService = taskService;
        }

        [HttpGet("learnertask")]
        public async Task<CourseTaskViewDetailDto> GetLearnerTask(Guid learnerTaskId)
        {
            return await _taskService.GetLearnerTask(learnerTaskId);
        }
    }
}
