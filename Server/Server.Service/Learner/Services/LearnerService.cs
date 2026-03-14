using AutoMapper;
using Common.Domain;
using Common.Repository;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Admin;
using Server.Domain.Learner;
using System.Linq.Expressions;

namespace Server.Service.Learner
{
    public class LearnerService(
        IDBRepository _repository,
        IMapper _mapper
        ) : ILearnerService
    {
        public async Task<TableInfo<ViewCourseDto>> GetCourseByUserIdPaging(CTableParameter parameter)
        {
            var query = new TableQueryParameter<MyCourseEntity>
            {
                Pager = new Pager { PageIndex = parameter.PageIndex, PageSize = parameter.PageSize },
                Sorter = GenerateSorter(parameter),
                Filter = GenerateFilter(parameter),
                CustomQuerable = s => s.Include(p => p.Course)
            };

            var result = await _repository.GetWithPagingAsync(query, entity => new ViewCourseDto()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                ProgressStatusEnum = entity.ProgressStatusEnum,
                ApplyStatus = entity.ApplyStatus,
                CreatedAt = entity.CreatedAt,
                ModifiedAt = entity.ModifiedAt,
                CourseId = entity.CourseId,
                CourseTitle = entity.Course.Title,
                Duration = entity.Course.Duration,
                CourseSummary = entity.Course.Summary,
                VideoURL = entity.Course.VideoURL,
                CourseStatus = entity.Course.Status,
                CourseType = entity.Course.CourseType,
                CategoryId = entity.Course.CategoryId,
                Image = entity.Course.Image,
            });

            var categoryIds = result.Items.Select(x => x.CategoryId).ToList();
            var DictCategory = (await _repository.GetAsync<CourseCategoryEntity>(p => categoryIds.Contains(p.Id)))
                            .ToDictionary(x => x.Id, x => x.Name);
            foreach (var mycourse in result.Items)
            {
                if (DictCategory.TryGetValue(mycourse.CategoryId, out var category))
                {
                    mycourse.CategoryName = category;
                }
            }

            return result;
        }

        private Expression<Func<MyCourseEntity, bool>> GenerateFilter(CTableParameter param)
        {
            var userId = RuntimeContext.Current.UserId;
            var filter = PredicateBuilder.True<MyCourseEntity>();
            filter = filter.And(p => !p.IsDeleted && p.UserId == userId);
            if (!string.IsNullOrWhiteSpace(param.SearchContent))
            {
                var content = SearchBuilder.BuildContent(param.SearchContent);
                filter = filter.And(p => EF.Functions.ILike(p.Course.Title, content.Pattern, content.EscapeCharacter));
            }

            if (param.Filters != null)
            {
                if (param.Filters.TryGetValue(nameof(ViewCourseDto.CategoryId), out var categoryIds) && categoryIds.Count == 1)
                {
                    var value = Guid.Parse(categoryIds[0]);
                    filter = filter.And(p => p.Course.CategoryId == value);
                }

                if (param.Filters.TryGetValue(nameof(ViewCourseDto.CourseType), out var courseTypes) && courseTypes.Count == 1)
                {
                    var value = (CourseType)Enum.Parse(typeof(CourseType), courseTypes[0]);
                    filter = filter.And(p => p.Course.CourseType == value);
                }
            }

            return filter;
        }

        private Sorter<MyCourseEntity, object> GenerateSorter(CTableParameter param)
        {
            var result = new Sorter<MyCourseEntity, object> { IsAscending = param.IsAscending };

            switch (param.SortKey ?? "")
            {
                default:
                    result.IsAscending = false;
                    result.SortBy = s => s.ModifiedAt;
                    break;
            }

            return result;
        }

        public async Task<MyCourseDto> GetDetail(Guid id)
        {
            var entity = await _repository.GetSet<MyCourseEntity>(p => p.Id == id)
                .Include(x => x.Course)
                .Include(x => x.UserQuizAttemps)
                .Include(x => x.TaskResults)
                .FirstOrDefaultAsync() ?? throw new NotExistException("MyCourse");


            var result = BaseConvert.MapBase<MyCourseDto>(entity);
            result.UserId = entity.UserId;
            result.ProgressStatusEnum = entity.ProgressStatusEnum;
            result.ApplyStatus = entity.ApplyStatus;
            result.CreatedAt = entity.CreatedAt;
            result.ModifiedAt = entity.ModifiedAt;

            result.Course = _mapper.Map<CourseDetailDto>(entity.Course);
            var category = await _repository.FindAsync<CourseCategoryEntity>(p => p.Id == entity.Course.CategoryId);
            result.Course.CategoryName = category?.Name;

            if (entity.Course.CourseType == CourseType.SimulationCourse)
            {
                result.TaskResults = entity.TaskResults.OrderBy(p => p.Order).Select(x => new TaskResultDto()
                {
                    Id = x.Id,
                    TaskId = x.TaskId,
                    TaskName = x.TaskName,
                    TaskSummary = x.TaskSummary,
                    IsCompleted = x.IsCompleted,
                    SubmittedAt = x.SubmittedAt,
                    SubmitAssignment = x.SubmitAssignment,
                }).ToList();

                result.Course.TaskCount = entity.TaskResults.Count;
                var completedTask = result.TaskResults.Count(p => p.IsCompleted);
                result.TaskProgress = (int)Math.Round(completedTask * 100.0 / result.TaskResults.Count, 2) ;
            }
            else if (entity.Course.CourseType == CourseType.CareerVideo)
            {
                if (entity.UserQuizAttemps != null)
                {
                    var quizResult = entity.UserQuizAttemps.FirstOrDefault();
                    if (quizResult != null)
                    {
                        result.IsSubmittedQuiz = quizResult.IsSubmitted;
                        result.QuizResult = $"{quizResult.Score}/{quizResult.TotalQuestion}";
                        result.Course.QuestionCount = quizResult.TotalQuestion;
                    }
                }
            }

            return result;
        }

        public async Task<bool> SubmitQuiz(SubmitQuizDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var mycourse = await _repository.GetSet<MyCourseEntity>(p => p.Id == dto.MyCourseId)
                .Include(p => p.UserQuizAttemps)
                .FirstOrDefaultAsync() ?? throw new NotExistException("MyCourse");

            if (dto.Questions?.Any() != true)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var questionIds = dto.Questions?.Select(q => q.QuestionId).ToList() ?? new List<Guid>();
            var answerIds = dto.Questions?.SelectMany(q => q.AnswerIds ?? new List<Guid>()).ToList() ?? new List<Guid>();

            var questions = mycourse.UserQuizAttemps
                .Where(q => q.Questions.Any(qs => questionIds.Contains(qs.Id)))
                .SelectMany(q => q.Questions)
                .ToList();
            foreach (var question in questions)
            {
                var correctIds = new HashSet<Guid>();
                var selectedIds = new HashSet<Guid>();

                foreach (var ans in question.Answers)
                {
                    if (ans == null) continue;

                    var isCorrectVal = ans.IsCorrect == true;
                    var isSelectedVal = answerIds.Contains(ans.Id);

                    if (isCorrectVal)
                        correctIds.Add(ans.Id);
                    if (isSelectedVal)
                    {
                        selectedIds.Add(ans.Id);
                        ans.IsSelected = true;
                    }
                }

                question.IsCorrect = correctIds.Count > 0 && correctIds.SetEquals(selectedIds);
            }

            // compute score and total explicitly and store back into dto
            var correctCount = questions.Count(q => q?.IsCorrect == true);
            dto.Score = correctCount;

            var userQuiz = mycourse.UserQuizAttemps.FirstOrDefault();
            userQuiz.Score = dto.Score;
            //userQuiz.TotalQuestion = dto.TotalQuestion;
            userQuiz.IsSubmitted = true;
            userQuiz.StartAt = dto.StartAt;
            userQuiz.EndAt = DateTimeOffset.UtcNow;
            userQuiz.Questions = questions;

            await _repository.ActionInTransaction(async () =>
            {
                await _repository.UpdateAsync(userQuiz);

                mycourse.ProgressStatusEnum = ProgressStatusEnum.Done;
                await _repository.UpdateAsync<MyCourseEntity>(mycourse);
            });

            return true;
        }

        public async Task<bool> SubmitTask(TaskResultDto dto)
        {
            if (dto.SubmitAssignment == null)
            {
                throw new WarningHandleException("Need to upload assignment");
            }

            var myCourse = await _repository.GetSet<MyCourseEntity>(p => p.Id == dto.MyCourseId)
                .Include(p => p.TaskResults)
                .FirstOrDefaultAsync() ?? throw new NotExistException("MyCourse");

            var learnerTask = myCourse.TaskResults.FirstOrDefault(p => p.Id == dto.Id) ?? throw new NotExistException("LearnerTask");

            if (learnerTask.IsCompleted)
            {
                throw new WarningHandleException("You has uploaded assignment");
            }

            learnerTask.SubmitAssignment = dto.SubmitAssignment;
            learnerTask.IsCompleted = true;
            learnerTask.SubmittedAt = DateTimeOffset.UtcNow;

            await _repository.ActionInTransaction(async () =>
            {
                var completedCount = myCourse.TaskResults.Count(p => p.IsCompleted);
                if (completedCount == myCourse.TaskResults.Count)
                {
                    myCourse.ProgressStatusEnum = ProgressStatusEnum.Done;
                    await _repository.UpdateAsync(myCourse);
                }
                await _repository.UpdateAsync(learnerTask);

            });

            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var entity = await _repository.FindAsync<MyCourseEntity>(p => p.Id == id) ?? throw new NotExistException("MyCourse");
            entity.ApplyStatus = ApplyStatus.ReApply;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
