using AutoMapper;
using Common.Domain;
using Common.Repository;
using Common.Service;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Admin;
using System.Linq.Expressions;

namespace Server.Service.Admin
{
    public class CourseManagementService(
        IDBRepository _repository,
        IMapper _mapper,
        IFileStorageService _fileStorageService
        ) : ICourseManagementService
    {
        public async Task<CourseDetailDto> GetDetail(Guid id)
        {
            var course = await _repository.GetSet<CourseEntity>(p => p.Id == id)
                .FirstOrDefaultAsync() ?? throw new NotExistException("Course");

            var result = new CourseDetailDto
            {
                Id = course.Id,
                Name = course.Name,
                CourseType = course.CourseType,
                Title = course.Title,
                Summary = course.Summary,
                Description = course.Description,
                Duration = course.Duration,
                CategoryId = course.CategoryId,
                Image = course.Image,
                VideoURL = course.VideoURL,
                Status = course.Status,
            };

            if (course.CourseType == CourseType.SimulationCourse)
            {
                var tasks = await _repository.GetSet<CourseTaskEntity>(p => p.CourseId == id).AsNoTracking().OrderBy(s => s.Order).ToListAsync();
                result.Tasks = _mapper.Map<List<CourseTaskDto>>(tasks);
            }
            else if (course.CourseType == CourseType.CareerVideo)
            {
                var questions = await _repository.GetSet<QuestionEntity>(p => p.CourseId == id).AsNoTracking().OrderBy(s => s.QuestionOrder).ToListAsync();
                result.Questions = _mapper.Map<List<QuestionDto>>(questions);
            }

            //result.CategoryName = (await _repository.GetSet<CourseCategoryEntity>(p => p.Id == course.CategoryId).FirstOrDefaultAsync())?.Name;

            return result;
        }

        public async Task<TableInfo<CourseDto>> GetPaging(CTableParameter parameter)
        {
            var query = new TableQueryParameter<CourseEntity>
            {
                Pager = new Pager { PageIndex = parameter.PageIndex, PageSize = parameter.PageSize },
                Sorter = GenerateSorter(parameter),
                Filter = GenerateFilter(parameter),
                CustomQuerable = s => s.Include(p => p.Tasks).Include(p => p.Questions)
            };

            var result = await _repository.GetWithPagingAsync(query, s => new CourseDto
            {
                Id = s.Id,
                Name = s.Name,
                Title = s.Title,
                Duration = s.Duration,
                CourseType = s.CourseType,
                Status = s.Status,
                CategoryId = s.CategoryId,
                TaskCount = s.Tasks.Count,
                QuizCount = s.Questions.Count,
                ModifiedAt = s.ModifiedAt
            });

            if (result.Items.Any())
            {
                var categoryIds = result.Items.Select(s => s.CategoryId).ToList();
                var categories = await _repository.GetSet<CourseCategoryEntity>(p => categoryIds.Contains(p.Id)).AsNoTracking().ToListAsync();

                foreach (var item in result.Items)
                {
                    item.CategoryName = categories.FirstOrDefault(p => p.Id == item.CategoryId)?.Name;
                }
            }

            return result;
        }

        public async Task<bool> Add(CourseDetailDto dto)
        {
            Validate(dto);

            var entity = new CourseEntity
            {
                Name = dto.Name,
                CourseType = dto.CourseType,
                Title = dto.Title,
                Summary = dto.Summary,
                Description = dto.Description,
                Duration = dto.Duration,
                VideoURL = dto.VideoURL,
                Status = CourseStatus.Draft,
                CategoryId = dto.CategoryId,
            };

            await _repository.ActionInTransaction(async () =>
            {
                await UpdateImage(dto, entity);

                if (entity.CourseType == CourseType.SimulationCourse)
                {
                    entity.Tasks = new List<CourseTaskEntity>();
                    foreach (var task in dto.Tasks)
                    {
                        var taskEntity = _mapper.Map<CourseTaskEntity>(task);
                        taskEntity.Id = Guid.NewGuid();
                        taskEntity.CourseId = entity.Id;

                        entity.Tasks.Add(taskEntity);
                    }
                }
                else if (entity.CourseType == CourseType.CareerVideo)
                {
                    entity.Questions = new List<QuestionEntity>();
                    foreach (var question in dto.Questions)
                    {
                        var questionEntity = _mapper.Map<QuestionEntity>(question);
                        questionEntity.Id = Guid.NewGuid();
                        questionEntity.CourseId = entity.Id;

                        entity.Questions.Add(questionEntity);
                    }
                }

                await _repository.AddAsync(entity);
            });

            return true;
        }

        public async Task<bool> Update(Guid id, CourseDetailDto dto)
        {
            Validate(dto);
            var query = _repository.GetSet<CourseEntity>(p => p.Id == id);

            if (dto.CourseType == CourseType.SimulationCourse)
            {
                query = query.Include(p => p.Tasks);
            }
            else
            {
                query = query.Include(p => p.Questions);
            }
            var course = await query.AsTracking().FirstOrDefaultAsync() ?? throw new NotExistException("Course");

            if (course.Status == CourseStatus.Published)
            {
                throw new DataValidationException("Can not update status", "", CErrorCode.StatusNotSupport);
            }

            course.Name = dto.Name;
            course.Title = dto.Title;
            course.Summary = dto.Summary;
            course.Description = dto.Description;
            course.Duration = dto.Duration;

            await _repository.ActionInTransaction(async () =>
            {
                await UpdateImage(dto, course);

                if (course.Status == CourseStatus.Draft)
                {
                    if (course.CourseType == CourseType.SimulationCourse && dto.Tasks.Any())
                    {
                        await UpdateTasks(dto.Tasks, course);
                    }
                    if (course.CourseType == CourseType.CareerVideo && dto.Questions.Any())
                    {
                        course.VideoURL = dto.VideoURL;
                        UpdateQuestions(dto.Questions, course);
                    }
                }

                await _repository.UpdateAsync(course);
            });

            return true;
        }

        public async Task<bool> Publish(Guid id)
        {
            return await UpdateStatus(id, CourseStatus.Published);
        }

        public async Task<bool> Unpublish(Guid id)
        {
            return await UpdateStatus(id, CourseStatus.Unpublished);
        }

        #region Private methods

        private async Task<bool> UpdateStatus(Guid id, CourseStatus status)
        {
            var course = await _repository.GetSet<CourseEntity>(p => p.Id == id).FirstOrDefaultAsync()
                ?? throw new NotExistException("Course");

            if (course.Status == status)
            {
                throw new DataValidationException("Can not update status", "", CErrorCode.StatusNotSupport);
            }

            course.Status = status;

            await _repository.UpdateAsync(course);
            return true;
        }

        private async Task UpdateImage(CourseDetailDto dto, CourseEntity entity)
        {
            if (entity.Image != null)
            {
                await _fileStorageService.DeleteDocumentFiles(new List<Guid> { entity.Image.Id });
            }

            if (dto.Image != null)
            {
                await _fileStorageService.SaveDocumentFile(dto.Image, entity.Id);
                entity.Image = dto.Image;
            }
        }

        private async Task UpdateTasks(List<CourseTaskDto> taskDtos, CourseEntity course)
        {
            course.Tasks ??= new List<CourseTaskEntity>();
            var incomingTaskIds = taskDtos.Where(t => t.Id != Guid.Empty).Select(t => t.Id).ToHashSet();

            // Remove tasks no longer in the list
            var tasksToRemove = course.Tasks.Where(t => !incomingTaskIds.Contains(t.Id)).ToList();
            if (tasksToRemove.Any())
            {
                await DeleteTaskDocuments(tasksToRemove);
                foreach (var i in tasksToRemove)
                {
                    course.Tasks.Remove(i);
                }
            }

            foreach (var taskDto in taskDtos)
            {
                if (taskDto.Id == Guid.Empty)
                {
                    var taskEntity = _mapper.Map<CourseTaskEntity>(taskDto);
                    taskEntity.CourseId = course.Id;
                    course.Tasks.Add(taskEntity);
                }
                else
                {
                    var existingTask = course.Tasks.FirstOrDefault(t => t.Id == taskDto.Id) ?? throw new NotExistException("Task");

                    existingTask.Name = taskDto.Name;
                    existingTask.Summary = taskDto.Summary;
                    existingTask.Introduce = taskDto.Introduce;
                    existingTask.Description = taskDto.Description;
                    existingTask.Duration = taskDto.Duration;
                    existingTask.Order = taskDto.Order;

                    existingTask.ExampleDocuments = taskDto.ExampleDocuments;
                    existingTask.SupportingDocuments = taskDto.SupportingDocuments;
                }
            }
        }

        private void UpdateQuestions(List<QuestionDto> questionDtos, CourseEntity course)
        {
            course.Questions ??= new List<QuestionEntity>();
            var incomingQuestionIds = questionDtos.Where(t => t.Id != Guid.Empty).Select(t => t.Id).ToHashSet();

            // Remove questions no longer in the list
            var questionsToRemove = course.Questions.Where(t => !incomingQuestionIds.Contains(t.Id)).ToList();
            if (questionsToRemove.Any())
            {
                foreach (var i in questionsToRemove)
                {
                    course.Questions.Remove(i);
                }
            }

            foreach (var questionDto in questionDtos)
            {
                if (questionDto.Id == Guid.Empty)
                {
                    var question = _mapper.Map<QuestionEntity>(questionDto);
                    question.CourseId = course.Id;
                    question.Answers.ForEach(i => i.Id = Guid.NewGuid());
                    course.Questions.Add(question);

                }
                else
                {
                    var existingQuestion = course.Questions.FirstOrDefault(t => t.Id == questionDto.Id) ?? throw new NotExistException("Question");
                    existingQuestion.QuestionOrder = questionDto.QuestionOrder;
                    existingQuestion.Name = questionDto.Name;
                    existingQuestion.QuestionType = questionDto.QuestionType;
                    existingQuestion.Answers = _mapper.Map<List<AnswerProperty>>(questionDto.Answers);
                    existingQuestion.Answers.ForEach(i => i.Id = Guid.NewGuid());
                }
            }
        }

        private async Task DeleteTaskDocuments(List<CourseTaskEntity> tasks)
        {
            var fileIds = tasks
                .SelectMany(t => (t.ExampleDocuments ?? new()).Concat(t.SupportingDocuments ?? new()))
                .Select(d => d.Id)
                .ToList();

            if (fileIds.Any())
            {
                await _fileStorageService.DeleteDocumentFiles(fileIds);
            }
        }

        private Expression<Func<CourseEntity, bool>> GenerateFilter(CTableParameter param)
        {
            var filter = PredicateBuilder.True<CourseEntity>();

            if (!string.IsNullOrWhiteSpace(param.SearchContent))
            {
                var content = param.SearchContent.ToLower().Trim();
                filter = filter.And(p => p.Name.ToLower().ToLower().Contains(content) || p.Title.ToLower().Contains(content));
            }

            if (param.Filters != null)
            {
                if (param.Filters.TryGetValue(nameof(CourseDto.Status), out var statusValues) && statusValues.Count == 1)
                {
                    var status = (CourseStatus)Enum.Parse(typeof(CourseStatus), statusValues[0]);
                    filter = filter.And(p => p.Status == status);
                }

                if (param.Filters.TryGetValue(nameof(CourseDto.CourseType), out var typeValues) && typeValues.Count == 1)
                {
                    var type = (CourseType)Enum.Parse(typeof(CourseType), typeValues[0]);
                    filter = filter.And(p => p.CourseType == type);
                }

                if (param.Filters.TryGetValue(nameof(CourseDto.CategoryId), out var categoryIdValues) && categoryIdValues.Any())
                {
                    var categoryIds = categoryIdValues.Select(s => Guid.Parse(s));
                    filter = filter.And(p => categoryIds.Contains(p.CategoryId));
                }
            }

            return filter;
        }

        private Sorter<CourseEntity, object> GenerateSorter(CTableParameter param)
        {
            var result = new Sorter<CourseEntity, object> { IsAscending = param.IsAscending };

            switch ((param.SortKey ?? "").ToLower())
            {
                case "title":
                    result.SortBy = s => s.Title;
                    break;
                case "coursetype":
                    result.SortBy = s => s.CourseType;
                    break;
                case "status":
                    result.SortBy = s => s.Status;
                    break;
                case "modifiedAt":
                    result.SortBy = s => s.ModifiedAt;
                    break;
                default:
                    result.SortBy = s => s.ModifiedAt;
                    break;
            }

            return result;
        }

        private void Validate(CourseDetailDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (dto.CourseType == CourseType.SimulationCourse)
            {
                if (dto.Tasks?.Any() != true)
                {
                    throw new DataValidationException("Tasks are required", "", CErrorCode.Required);
                }
                else
                {
                    var currentOrder = 1;
                    var tasks = dto.Tasks.OrderBy(p => p.Order);
                    foreach (var task in tasks)
                    {
                        if (task.Order != currentOrder)
                        {
                            throw new DataValidationException("Task order is incorrect", "", CErrorCode.InvalidInput);
                        }
                        currentOrder++;
                    }
                }
            }

            if (dto.CourseType == CourseType.CareerVideo)
            {
                if (dto.Questions?.Any() != true)
                {
                    throw new DataValidationException("Questions are required", "", CErrorCode.Required);
                }
                else
                {
                    var currentOrder = 1;
                    var questions = dto.Questions.OrderBy(p => p.QuestionOrder);
                    foreach (var question in questions)
                    {
                        if (question.QuestionOrder != currentOrder)
                        {
                            throw new DataValidationException("Question order is incorrect", "", CErrorCode.InvalidInput);
                        }
                        currentOrder++;

                        if (question.Answers?.Any() != true)
                        {
                            throw new DataValidationException("Answers are required", "", CErrorCode.Required);
                        }

                        var answerOrder = 1;
                        var answers = question.Answers.OrderBy(p => p.Order);
                        var correctAnswerCount = 0;
                        foreach (var ans in answers)
                        {
                            if (ans.Order != answerOrder)
                            {
                                throw new DataValidationException("Answer order is incorrect", "", CErrorCode.InvalidInput);
                            }
                            if (ans.IsCorrect)
                            {
                                correctAnswerCount++;
                            }
                            answerOrder++;
                        }

                        if (correctAnswerCount == 0 || (question.QuestionType == CQuestionType.SingleChoice && correctAnswerCount > 1))
                        {
                            throw new DataValidationException("Correct answer is invalid", "", CErrorCode.InvalidInput);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
