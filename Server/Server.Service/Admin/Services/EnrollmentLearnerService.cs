using Common.Domain;
using Common.Repository;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Admin;
using System.Linq.Expressions;

namespace Server.Service.Admin
{
    public class EnrollmentLearnerService(IDBRepository _repository)
        : IEnrollmentLearnerService
    {
        public async Task<TableInfo<EnrollmentLearnerDto>> GetPaging(CTableParameter parameter, Guid courseId)
        {
            var query = new TableQueryParameter<MyCourseEntity>
            {
                Pager = new Pager { PageIndex = parameter.PageIndex, PageSize = parameter.PageSize },
                Sorter = GenerateSorter(parameter),
                Filter = GenerateFilter(parameter)
            };

            var result = await _repository.GetWithPagingAsync(query, s => new EnrollmentLearnerDto
            {
                Id = s.Id,
                LearnerId = s.UserId,
                LearnerID = s.Learner.LearnerID,
                LearnerName = s.Learner.Account.Name,
                ProgressStatus = s.ProgressStatusEnum,
                EnrolledTime = s.CreatedAt
            });

            return result;
        }

        public async Task<LearnerEnrollmentResultDto> GetEnrollmentResult(Guid myCourseId)
        {
            var myCourse = await _repository.GetSet<MyCourseEntity>(p => p.CourseId == myCourseId)
                .Select(s => new
                {
                    s.CourseId,
                    s.Course.Name,
                    s.Course.CourseType,
                    s.Learner.LearnerID,
                }).FirstOrDefaultAsync() ?? throw new NotExistException("My course");

            var result = new LearnerEnrollmentResultDto
            {
                CourseId = myCourseId,
                CourseType = myCourse.CourseType,
                LearnerID = myCourse.LearnerID,
            };


            if (myCourse.CourseType == CourseType.SimulationCourse)
            {
                var courseTaskQuery = from task in _repository.GetSet<CourseTaskEntity>(p => p.CourseId == myCourse.CourseId)
                                  join learnerTask in _repository.GetSet<TaskResultEntity>(p => p.MyCourseId == myCourseId)
                                  on task.Id equals learnerTask.TaskId into LT
                                  from learnerTask in LT.DefaultIfEmpty()
                                  select new
                                  {
                                      task.Id,
                                      task.Name,
                                      learnerTask.SubmitAssignment,
                                      learnerTask.SubmittedAt,
                                      learnerTask.FeedBacks,
                                  };
                var courseTasks = await courseTaskQuery.ToListAsync();
                result.LearnerTasks = new List<LearnerTaskDto>();
                foreach (var courseTask in courseTasks)
                {
                    var task = new LearnerTaskDto
                    {
                        TaskId = courseTask.Id,
                        TaskName = courseTask.Name,
                        CourseType = result.CourseType,
                        SubmittedAt = courseTask.SubmittedAt,
                        SubmitedAssignment = courseTask.SubmitAssignment,
                        FeedBacks = courseTask.FeedBacks.Select(s => new LearnerFeedBackDto
                        {
                            Id = s.Id,
                            Title = s.Title,
                            Content = s.Content,
                            SendOn = s.CreatedAt,
                        }).ToList(),
                    };

                    result.LearnerTasks.Add(task);
                }
            }
            else if (myCourse.CourseType == CourseType.CareerVideo)
            {
                 // TODO
            }

            return result;
        }

        public async Task<bool> SendFeedback(FeedBackDto feedBackDto)
        {
            var learnerTask = await _repository.GetSet<TaskResultEntity>(p => p.MyCourseId == feedBackDto.MyCourseId && p.TaskId == feedBackDto.TaskId)
                .Select(s => new
                {
                    Id = s.Id,
                    UserId = s.MyCourse.UserId,
                    TaskName = s.CourseTask.Name,
                })
                .FirstOrDefaultAsync()
                ?? throw new NotExistException("MyCourse");

            var feedBack = new FeedBackEntity
            {
                Title = $"Feedback: {learnerTask.TaskName}",
                Content = feedBackDto.Content,
                LearnerTaskId = learnerTask.Id,
                UserId = learnerTask.UserId,
            };

            await _repository.AddAsync(feedBack);
            return true;
        }

        private Expression<Func<MyCourseEntity, bool>> GenerateFilter(CTableParameter param)
        {
            var filter = PredicateBuilder.True<MyCourseEntity>();

            if (!string.IsNullOrWhiteSpace(param.SearchContent))
            {
                var content = param.SearchContent.ToLower().Trim();
                filter = filter.And(p => p.Learner.LearnerID.ToLower().Contains(content));
            }

            return filter;
        }

        private Sorter<MyCourseEntity, object> GenerateSorter(CTableParameter param)
        {
            var result = new Sorter<MyCourseEntity, object> { IsAscending = param.IsAscending };
            switch ((param.SortKey ?? "").ToLower())
            {
                case "learnerid":
                    result.SortBy = s => s.Learner.LearnerID;
                    break;
                case "enrolledtime":
                    result.SortBy = s => s.CreatedAt;
                    break;
                case "name":
                    result.SortBy = s => s.Learner.Account.Name;
                    break;
            }
            return result;
        }
    }
}
