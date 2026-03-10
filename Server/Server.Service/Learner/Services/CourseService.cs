using Common.Domain;
using Common.Repository;
using Common.Service;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Admin;
using Server.Domain.Learner;
using System.Linq.Expressions;

namespace Server.Service.Learner
{
    public class CourseService(
        IDBRepository _repository
        ) : ICourseService
    {
        public async Task<TableInfo<ViewCourseDto>> GetPaging(CTableParameter parameter, CourseType courseType)
        {
            var query = new TableQueryParameter<CourseEntity>
            {
                Pager = new Pager { PageIndex = parameter.PageIndex, PageSize = parameter.PageSize },
                Sorter = GenerateSorter(parameter),
                Filter = GenerateFilter(parameter, courseType),
                CustomQuerable = s => s.Include(p => p.Tasks)
            };

            var result = await _repository.GetWithPagingAsync(query, entity => new ViewCourseDto()
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                ModifiedAt = entity.ModifiedAt,
                CourseTitle = entity.Title,
                Duration = entity.Duration,
                CourseSummary = entity.Summary,
                VideoURL = entity.VideoURL,
                CourseStatus = entity.Status,
                CourseType = entity.CourseType,
                CategoryId = entity.CategoryId,
                TaskCount = entity.Tasks.Count,
                Image = entity.Image,
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

        private Expression<Func<CourseEntity, bool>> GenerateFilter(CTableParameter param, CourseType courseType)
        {
            var userId = RuntimeContext.Current.UserId;
            var filter = PredicateBuilder.True<CourseEntity>();
            filter = filter.And(p => !p.IsDeleted && p.CourseType == courseType && p.Status == CourseStatus.Published);
            if (!string.IsNullOrWhiteSpace(param.SearchContent))
            {
                var content = SearchBuilder.BuildContent(param.SearchContent);
                filter = filter.And(p => EF.Functions.ILike(p.Name, content.Pattern, content.EscapeCharacter));
            }
            return filter;
        }

        private Sorter<CourseEntity, object> GenerateSorter(CTableParameter param)
        {
            var result = new Sorter<CourseEntity, object> { IsAscending = param.IsAscending };

            switch (param.SortKey ?? "")
            {
                default:
                    result.SortBy = s => s.Name;
                    break;
            }

            return result;
        }

        private async Task EntityMapAsync(List<MyCourseDto> result)
        {
            //var categoryIds = result.Select(x => x.Course.CategoryId).ToList();
            //var DictCategory = (await _repository.GetAsync<CourseCategoryEntity>(p => categoryIds.Contains(p.Id)))
            //                .ToDictionary(x => x.Id, x => x.Name);

            var myCourseIds = result.Select(x => x.Id).ToList();
            var userQuizAttempts = await _repository.GetAsync<UserQuizAttempEntity>(
                x => myCourseIds.Contains(x.MyCourseId));
            var userAttemptLookup = userQuizAttempts
                .GroupBy(x => x.MyCourseId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var attemptIds = userQuizAttempts.Select(x => x.Id).ToList();
            //var userAnswers = await _repository.GetAsync<UserAnswerEntity>(
            //    x => attemptIds.Contains(x.UserQuizAttempId));
            //var answerLookup = userAnswers
            //    .GroupBy(x => x.UserQuizAttempId)
            //    .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var mycourse in result)
            {
                //if (DictCategory.TryGetValue(mycourse.Course.Id, out var category))
                //{
                //    mycourse.Course.CategoryName = category;
                //}

                if (mycourse.Course.CourseType == CourseType.SimulationCourse)
                {
                    if (!userAttemptLookup.TryGetValue(mycourse.Id, out var userAttemps))
                        continue;

                    //mycourse.UserQuizAttempDtos = userAttemps.Select(a =>
                    //{
                    //    answerLookup.TryGetValue(a.Id, out var answers);

                    //    return new UserQuizAttempDto
                    //    {
                    //        Id = a.Id,
                    //        MyCourseId = a.MyCourseId,
                    //        TotalScore = a.TotalScore,
                    //        StartAt = a.StartAt,
                    //        EndAt = a.EndAt,
                    //        Progress = a.Progress,

                    //        UserAnswerDtos = answers?.Select(ans => new UserAnswerDto
                    //        {
                    //            Id = ans.Id,
                    //            UserAttempId = ans.UserQuizAttempId,
                    //            QuestionId = ans.QuestionId,
                    //            AnswerId = ans.AnswerId
                    //        }).ToList() ?? new List<UserAnswerDto>()
                    //    };
                    //}).ToList();
                }
            }
        }

        public async Task<MyCourseDto> GetDetail(Guid id)
        {
            var entity = await _repository.FindAsync<MyCourseEntity>(p => p.Id == id) ?? throw new NotExistException("MyCourse");
            var category = await _repository.FindAsync<CourseCategoryEntity>(p => p.Id == entity.Course.CategoryId) ?? throw new NotExistException("Category");

            var result = BaseConvert.MapBase<MyCourseDto>(entity);
            result.UserId = entity.UserId;
            result.ProgressStatusEnum = entity.ProgressStatusEnum;
            result.ApplyStatus = entity.ApplyStatus;
            result.CreatedAt = entity.CreatedAt;
            result.ModifiedAt = entity.ModifiedAt;
            //result.TaskResultDtos = entity.TaskResults.Select(x => new TaskResultDto()
            //{
            //    Id = x.Id,
            //    TaskId = x.TaskId,
            //    MyCourseId = x.MyCourseId,
            //    SubmittedAt = x.SubmittedAt,
            //    SubmitAssignment = x.SubmitAssignment,
            //}).ToList();

            result.Course = BaseConvert.MapBase<CourseDetailDto>(entity.Course);
            result.Course.Title = entity.Course.Title;
            result.Course.Name = entity.Course.Name;
            result.Course.Description = entity.Course.Description;
            result.Course.Summary = entity.Course.Summary;
            result.Course.Duration = entity.Course.Duration;
            result.Course.CourseType = entity.Course.CourseType;
            result.Course.Status = entity.Course.Status;
            result.Course.CategoryId = entity.Course.CategoryId;
            result.Course.CategoryName = category?.Name;
            result.Course.TaskCount = entity.Course.Tasks.Count;
            //result.Course.Tasks = entity.Course.Tasks.Select(x =>
            //{
            //    var dto = BaseConvert.MapBase<CourseTaskDto>(x);
            //    dto.Name = x.Name;
            //    dto.Summary = x.Summary;
            //    dto.Introduce = x.Introduce;
            //    dto.Description = x.Description;
            //    dto.Duration = x.Duration;
            //    dto.Order = x.Order;
            //    dto.SupportingDocuments = x.SupportingDocuments;
            //    dto.ExampleDocuments = x.ExampleDocuments;
            //    return dto;

            //}).ToList();
            //result.Course.Questions = entity.Course.Questions.Select(x => QuizConvert.QuestionConvertToDto(x)).ToList();

            var mycourses = new List<MyCourseDto>() { result };
            //await EntityMapAsync(mycourses);

            return result;
        }

        public async Task<bool> Apply(MyCourseCreateDto dto)
        {
            var entity = await _repository.FindAsync<MyCourseEntity>(p => p.Id == dto.Id);
            if (entity != null)
            {
                entity.ApplyStatus = ApplyStatus.Apply;
                await _repository.UpdateAsync(entity);
            }
            else
            {
                var mycourse = new MyCourseEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.UserId,
                    CourseId = dto.CourseId,
                    ProgressStatusEnum = ProgressStatusEnum.None,
                    ApplyStatus = ApplyStatus.Apply,
                };
                await _repository.AddAsync(mycourse);
            }

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
