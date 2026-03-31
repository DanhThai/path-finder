using AutoMapper;
using Common.Domain;
using Common.Repository;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Admin;
using Server.Domain.Learner;
using System.Linq.Expressions;

namespace Server.Service.Learner
{
    public class CourseService(
        IDBRepository _repository,
        IMapper _mapper
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

            var courseIds = result.Items.Select(p => p.Id).Distinct();
            var appliedCourseDict = new Dictionary<Guid, bool>();
            if (courseIds.Any())
            {
                appliedCourseDict = await _repository.GetSet<MyCourseEntity>(p => p.UserId == RuntimeContext.Current.UserId && courseIds.Contains(p.CourseId))
                    .Select(s => new { s.Id, s.CourseId })
                    .ToDictionaryAsync(k => k.CourseId, v => true);
            }

            foreach (var mycourse in result.Items)
            {
                mycourse.HasApplied = appliedCourseDict.GetValueOrDefault(mycourse.Id);

                if (DictCategory.TryGetValue(mycourse.CategoryId, out var category))
                {
                    mycourse.CategoryName = category;
                }
            }
            return result;
        }
        
        public async Task<List<ViewCourseDto>> GetPopularCourses()
        {
            var courses = await _repository.GetSet<CourseEntity>(p => p.Status == CourseStatus.Published && !p.IsDeleted)
                .Select(s => new ViewCourseDto
                {
                    Id = s.Id,
                    CreatedAt = s.CreatedAt,
                    ModifiedAt = s.ModifiedAt,
                    CourseTitle = s.Title,
                    Duration = s.Duration,
                    CourseSummary = s.Summary,
                    VideoURL = s.VideoURL,
                    CourseStatus = s.Status,
                    CourseType = s.CourseType,
                    CategoryId = s.CategoryId,
                    TaskCount = s.Tasks.Count,
                    Image = s.Image,
                    EnrolledCount = s.EnrolledLearners.Count,
                })
                .OrderByDescending(k => k.EnrolledCount).ThenBy(k => k.CourseTitle)
                .Take(3)
                .ToListAsync();

            return courses;
        }

        private Expression<Func<CourseEntity, bool>> GenerateFilter(CTableParameter param, CourseType courseType)
        {
            var userId = RuntimeContext.Current.UserId;
            var filter = PredicateBuilder.True<CourseEntity>();
            filter = filter.And(p => !p.IsDeleted && p.CourseType == courseType && p.Status == CourseStatus.Published);
            if (!string.IsNullOrWhiteSpace(param.SearchContent))
            {
                var content = SearchBuilder.BuildContent(param.SearchContent);
                filter = filter.And(p => EF.Functions.ILike(p.Title, content.Pattern, content.EscapeCharacter));
            }

            if (param.Filters != null)
            {
                if (param.Filters.TryGetValue(nameof(CourseEntity.CategoryId), out var categoryId) && categoryId.Count == 1)
                {
                    filter = filter.And(p => p.CategoryId == new Guid(categoryId[0]));
                }
            }
            return filter;
        }

        private Sorter<CourseEntity, object> GenerateSorter(CTableParameter param)
        {
            var result = new Sorter<CourseEntity, object> { IsAscending = param.IsAscending };

            switch (param.SortKey ?? "")
            {
                default:
                    result.IsAscending = false;
                    result.SortBy = s => s.ModifiedAt;
                    break;
            }

            return result;
        }

        public async Task<CourseDetailDto> GetDetail(Guid id)
        {
            var userId = RuntimeContext.Current.UserId;
            var entity = await _repository.GetSet<CourseEntity>(p => p.Id == id)
                .Include(x => x.Tasks)
                .Include(x => x.Questions)
                .FirstOrDefaultAsync() ?? throw new NotExistException("Course");
            var category = await _repository.FindAsync<CourseCategoryEntity>(p => p.Id == entity.CategoryId) ?? throw new NotExistException("Category");

            var result = new CourseDetailDto();
            result = BaseConvert.MapBase<CourseDetailDto>(entity);
            result.Title = entity.Title;
            result.Name = entity.Name;
            result.Description = entity.Description;
            result.Summary = entity.Summary;
            result.Duration = entity.Duration;
            result.VideoURL = entity.VideoURL;
            result.CourseType = entity.CourseType;
            result.Status = entity.Status;
            result.CategoryId = entity.CategoryId;
            result.CategoryName = category?.Name;
            result.TaskCount = entity.Tasks.Count;
            result.QuestionCount = entity.Questions.Count;
            result.Image = entity.Image;

            var learnerCourse = await _repository.GetSet<MyCourseEntity>(p => p.CourseId == id && p.UserId == RuntimeContext.Current.UserId)
                .Select(p => new { p.Id })
                .FirstOrDefaultAsync();
            if (learnerCourse != null)
            {
                result.HasApplied = true;
                result.MyCourseId = learnerCourse.Id;
            }

            return result;
        }

        public async Task<Guid> Apply(MyCourseCreateDto dto)
        {
            var course = await _repository.GetSet<CourseEntity>(p => p.Id == dto.CourseId)
                .Include(p => p.Tasks)
                .Include(p => p.Questions)
                .FirstOrDefaultAsync()
                ?? throw new NotExistException("Course");

            var existingMyCourse = await _repository.GetSet<MyCourseEntity>(p => p.CourseId == dto.CourseId && p.UserId == RuntimeContext.Current.UserId)
                .Select(s => new { s.Id})
                .FirstOrDefaultAsync();
            if (existingMyCourse != null)
            {
                return existingMyCourse.Id;
            }
            else
            {
                var newMyCourse = new MyCourseEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = RuntimeContext.Current.UserId,
                    CourseId = dto.CourseId,
                    ProgressStatusEnum = ProgressStatusEnum.Inprogress,
                    ApplyStatus = ApplyStatus.Apply,
                };

                if (course.CourseType == CourseType.SimulationCourse)
                {
                    var tasks = new List<TaskResultEntity>();

                    if (course.Tasks != null)
                    {
                        foreach (var task in course.Tasks)
                        {
                            tasks.Add(new TaskResultEntity
                            {
                                MyCourseId = newMyCourse.Id,
                                TaskId = task.Id,
                                TaskName = task.Name,
                                TaskSummary = task.Summary,
                                Order = task.Order
                            });
                        }
                    }

                    newMyCourse.TaskResults = tasks;
                }
                else if (course.CourseType == CourseType.CareerVideo)
                {
                    var quiz = new List<UserQuizAttempEntity>();

                    if (course.Questions != null)
                    {
                        quiz.Add(new UserQuizAttempEntity
                        {
                            MyCourseId = newMyCourse.Id,
                            TotalQuestion = course.Questions.Count,
                            Questions = _mapper.Map<List<QuestionProperty>>(course.Questions.ToList())
                        });
                    }

                    newMyCourse.UserQuizAttemps = quiz;
                }

                await _repository.AddAsync(newMyCourse);
                return newMyCourse.Id;
            }
        }
    }
}
