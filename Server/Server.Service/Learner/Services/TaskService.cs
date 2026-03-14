using AutoMapper;
using Common.Domain;
using Common.Repository;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Admin;
using Server.Domain.Learner;

namespace Server.Service.Learner
{
    public class TaskService(
        IDBRepository _repository,
        IMapper _mapper
        ) : ITaskService
    {
        public async Task<List<CourseTaskViewDetailDto>> GetTaskByCourseId(Guid courseId)
        {
            var courseTasks = await _repository
                    .GetSet<CourseTaskEntity>(x => x.CourseId == courseId)
                    .Include(x => x.Course)
                    .ToListAsync();
            var taskIds = courseTasks.Select(x => x.Id).ToList();
            var taskResults = await _repository
                    .GetSet<TaskResultEntity>(x => taskIds.Contains(x.TaskId))
                    .Include(x => x.MyCourse)
                    .Include(x => x.FeedBacks)
                    .ToListAsync();

            var result = new List<CourseTaskViewDetailDto>();
            foreach (var courseTask in courseTasks)
            {
                var taskDto = _mapper.Map<CourseTaskViewDetailDto>(courseTask);

                var taskResult = taskResults.FirstOrDefault(x => x.TaskId == courseTask.Id);
                taskDto.ProgressStatusEnum = taskResult.MyCourse.ProgressStatusEnum;
                taskDto.Feedbacks = taskResult.FeedBacks != null
                    ? _mapper.Map<List<LearnerFeedbackDto>>(taskResult.FeedBacks)
                    : new List<LearnerFeedbackDto>();

                result.Add(taskDto);
            }

            return result;
        }

        public async Task<CourseTaskViewDetailDto> GetLearnerTask(Guid learnerTaskId)
        {
            var taskResult = await _repository.GetSet<TaskResultEntity>(p => p.Id == learnerTaskId)
                .Include(p => p.CourseTask)
                .Include(p => p.FeedBacks)
                .FirstOrDefaultAsync() ?? throw new NotExistException("TaskResult");

            var result = _mapper.Map<CourseTaskViewDetailDto>(taskResult.CourseTask);
            result.Id = taskResult.Id;
            result.MyCourseId = taskResult.MyCourseId;
            result.SubmittedAt = taskResult.SubmittedAt;
            result.SubmitAssignment = taskResult.SubmitAssignment;

            if (taskResult.FeedBacks != null)
            {
                result.Feedbacks = new List<LearnerFeedbackDto>();
                var userIds = taskResult.FeedBacks.Select(p => p.CreatedBy).Distinct();
                var userDict = await _repository.GetSet<AccountEntity>(p => userIds.Contains(p.Id))
                    .Select(s => new { s.Id, s.Name })
                    .ToDictionaryAsync(k => k.Id, v => v.Name);

                foreach (var item in taskResult.FeedBacks)
                {
                    var feedBack = _mapper.Map<LearnerFeedbackDto>(item);
                    feedBack.SendOn = item.CreatedAt;
                    feedBack.SendBy = userDict.GetValueOrDefault(item.CreatedBy);
                    result.Feedbacks.Add(feedBack);
                }
            }

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
