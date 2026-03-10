using AutoMapper;
using Common.Domain;
using Common.Repository;
using Common.Service;
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
                                        .Include(p => p.TaskResults)
                //.Include(p => p.TaskResults)
                //.Include(p => p.User)
                //.Include(p => p.UserQuizAttemps)
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
                TaskCount = entity.Course.Tasks.Count,
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
            //await EntityMapAsync(result.Items);
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
                filter = filter.And(p => EF.Functions.ILike(p.Course.Name, content.Pattern, content.EscapeCharacter));
            }
            return filter;
        }

        private Sorter<MyCourseEntity, object> GenerateSorter(CTableParameter param)
        {
            var result = new Sorter<MyCourseEntity, object> { IsAscending = param.IsAscending };

            switch (param.SortKey ?? "")
            {
                default:
                    result.SortBy = s => s.Course.Name;
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
            result.TaskResultDtos = entity.TaskResults.Select(x => new TaskResultDto()
            {
                Id = x.Id,
                TaskId = x.TaskId,
                MyCourseId = x.MyCourseId,
                SubmittedAt = x.SubmittedAt,
                SubmitAssignment = x.SubmitAssignment,
            }).ToList();

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
            result.Course.Tasks = entity.Course.Tasks.Select(x =>
            {
                var dto = BaseConvert.MapBase<CourseTaskDto>(x);
                dto.Name = x.Name;
                dto.Summary = x.Summary;
                dto.Introduce = x.Introduce;
                dto.Description = x.Description;
                dto.Duration = x.Duration;
                dto.Order = x.Order;
                dto.SupportingDocuments = x.SupportingDocuments;
                dto.ExampleDocuments = x.ExampleDocuments;
                return dto;

            }).ToList();
            result.Course.Questions = entity.Course.Questions.Select(x => QuizConvert.QuestionConvertToDto(x)).ToList();

            var mycourses = new List<MyCourseDto>() { result };
            await EntityMapAsync(mycourses);

            return result;
        }

        public async Task<bool> SubmitQuiz(SubmitQuizDto dto)
        {
            var userQuizAttemp = new UserQuizAttempEntity
            {
                Id = Guid.NewGuid(),
                MyCourseId = dto.MyCourseId,
                Progress = dto.Progress,
                Score = dto.Score,
                TotalQuestion = dto.TotalQuestion,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                Questions = dto.Questions.Select(x =>
                {
                    var question = _mapper.Map<QuestionEntity>(x);
                    question.Answers = _mapper.Map<List<AnswerProperty>>(question.Answers);
                    return question;
                }).ToList()
            };
            var mycourse = await _repository.FindAsync<MyCourseEntity>(p => p.Id == dto.MyCourseId) ?? throw new NotExistException("MyCourse");
            mycourse.ProgressStatusEnum = ProgressStatusEnum.Done;
            
            await _repository.AddAsync(userQuizAttemp);
            await _repository.UpdateAsync<MyCourseEntity>(mycourse);

            return true;
        }

        public async Task<bool> SubmitTask(TaskResultDto dto)
        {
            var taskResult = new TaskResultEntity
            {
                Id = Guid.NewGuid(),
                TaskId = dto.TaskId,
                MyCourseId = dto.MyCourseId,
                SubmittedAt = dto.SubmittedAt,
                SubmitAssignment = dto.SubmitAssignment
            };

            await _repository.AddAsync(taskResult);

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
