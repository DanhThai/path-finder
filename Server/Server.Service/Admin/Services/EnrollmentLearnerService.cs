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
                Filter = GenerateFilter(parameter, courseId)
            };

            var result = await _repository.GetWithPagingAsync(query, s => new EnrollmentLearnerDto
            {
                Id = s.Id,
                //LearnerId = s.UserId,
                LearnerID = s.Learner.LearnerID,
                LearnerName = s.Learner.Account.Name,
                ProgressStatus = s.ProgressStatusEnum,
                EnrolledTime = s.CreatedAt
            });

            return result;
        }

        public async Task<LearnerEnrollmentResultDto> GetEnrollmentResult(Guid myCourseId)
        {
            var myCourse = await _repository.GetSet<MyCourseEntity>(p => p.Id == myCourseId)
                .Select(s => new
                {
                    s.Id,
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
                                  join learnerTask in _repository.GetSet<TaskResultEntity>(p => p.MyCourseId == myCourse.Id)
                                  on task.Id equals learnerTask.TaskId into LT
                                  from learnerTask in LT.DefaultIfEmpty()
                                  select new
                                  {
                                      task.Id,
                                      task.Name,
                                      task.Order,
                                      learnerTask.SubmitAssignment,
                                      learnerTask.SubmittedAt,
                                      learnerTask.FeedBacks,
                                  };
                var courseTasks = await courseTaskQuery.OrderBy(p => p.Order).ToListAsync();
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
                        FeedBacks = courseTask.FeedBacks.OrderByDescending(p => p.CreatedAt).Select(s => new UserFeedBackDto
                        {
                            Id = s.Id,
                            Title = s.Title,
                            Content = s.Content,
                            SendOn = s.CreatedAt,
                            SendById = s.CreatedBy,
                        }).ToList(),
                    };

                    if (courseTask.FeedBacks?.Any() == true)
                    {
                        var sendByIds = courseTask.FeedBacks.Select(s => s.CreatedBy).Distinct().ToList();
                        var userDict = await _repository.GetSet<AccountEntity>(p => sendByIds.Contains(p.Id))
                            .Select(s => new { s.Id, s.Name })
                            .ToDictionaryAsync(k => k.Id, v => v.Name);

                        task.FeedBacks.ForEach(i => i.SendBy = userDict.GetValueOrDefault(i.SendById));
                    }

                    result.LearnerTasks.Add(task);
                }
            }
            else if (myCourse.CourseType == CourseType.CareerVideo)
            {
                 var quizResult = await _repository.GetSet<UserQuizAttempEntity>(p => p.MyCourseId == myCourse.Id)
                    .Select(s => new
                    {
                        s.IsSubmitted,
                        s.Score,
                        s.TotalQuestion,
                        s.CreatedAt,
                        s.Questions,
                    }).FirstOrDefaultAsync();

                if (quizResult != null && quizResult.IsSubmitted)
                {
                    result.QuizResult = $"{quizResult.Score}/{quizResult.TotalQuestion}";

                    if (quizResult.Questions != null)
                    {
                        result.QuestionResults = new List<LearnerQuestionResultDto>();

                        foreach (var question in quizResult.Questions.OrderBy(p => p.QuestionOrder))
                        {
                            var answers = question.Answers.Select(s => new LearnerAnswerDto
                            {
                                Order = s.Order,
                                Name = s.Name,
                                ExplainDescription = s.ExplainDescription,
                                IsCorrect = s.IsCorrect,
                                IsSelected = s.IsSelected ?? false,
                            }).ToList();

                            result.QuestionResults.Add(new LearnerQuestionResultDto
                            {
                                QuestionId = question.Id,
                                QuestionName = question.Name,
                                QuestionOrder = question.QuestionOrder,
                                QuestionType = question.QuestionType,
                                Answers = answers.OrderBy(p => p.Order).ToList()
                            });
                        }
                    }
                }
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

        private Expression<Func<MyCourseEntity, bool>> GenerateFilter(CTableParameter param, Guid courseId)
        {
            var filter = PredicateBuilder.True<MyCourseEntity>();
            filter = filter.And(p => p.CourseId == courseId);

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
                default:
                    result.IsAscending = false;
                    result.SortBy = s => s.CreatedAt;
                    break;
            }
            return result;
        }
    }
}
