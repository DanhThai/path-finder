using AutoMapper;
using Common.Domain;
using Common.Repository;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Learner;

namespace Server.Service.Learner
{
    public class LearnerProfileService(
        IDBRepository _repository
        ) : ILearnerProfileService
    {

        public async Task<LearnerProfileDto> GetMyProfile()
        {
            var userId = RuntimeContext.Current.UserId;
            var learner = await _repository.GetSet<LearnerProfileEntity>(p => p.Id == userId)
                .Select(s => new LearnerProfileDto
                {
                    Id = s.Id,
                    Name = s.Account.Name,
                    Email = s.Account.Email,
                    LearnerID = s.LearnerID,
                    MajorId = s.MajorId,
                    Avatar = s.Account.Avatar,
                })
                .FirstOrDefaultAsync() ?? throw new NotExistException("LearnerProfile");
            if (learner.MajorId != Guid.Empty)
            {
                var major = await _repository.GetSet<LearnerMajorEntity>(p => p.Id == learner.MajorId).FirstOrDefaultAsync();
                learner.MajorName = major?.Name;
            }
            else
            {
                learner.MajorId = null;
            }

            return learner;
        }

        public async Task<ProfileDashBoardDto> GetMyProfileDashBoard()
        {
            var result = new ProfileDashBoardDto();
            var userId = RuntimeContext.Current.UserId;
            var myCourses = await _repository
                    .GetSet<MyCourseEntity>(x => x.UserId == userId)
                    .Include(x => x.Course)
                    .Include(x => x.UserQuizAttemps)
                    .Include(x => x.TaskResults)
                    .ToListAsync();
            var countUserAttemp = myCourses.SelectMany(x => x.UserQuizAttemps).Count(p => p.IsSubmitted);
            var countTaskResult = myCourses.SelectMany(x => x.TaskResults).Count(p => p.IsCompleted);
            var quizzes = new List<UserQuizDashBoardDto>();
            var tasks = new List<CourseTaskDashBoardDto>();
            var feedbackEntities = await _repository.GetAsync<FeedBackEntity>(p => p.UserId == userId) ?? new List<FeedBackEntity>();
            var feedbacks = feedbackEntities.Select(fe => new LearnerFeedbackDto
            {
                Id = fe.Id,
                learnerTaskId = fe.LearnerTaskId,
                Title = fe.Title,
                Content = fe.Content,
                UserId = fe.UserId,
                CreatedAt = fe.CreatedAt
            }).ToList();

            foreach (var myCourse in myCourses)
            {
                var userQuiz = myCourse.UserQuizAttemps.FirstOrDefault();
                if (userQuiz != null && userQuiz.IsSubmitted)
                {
                    var quiz = new UserQuizDashBoardDto()
                    {
                        MyCourseId = userQuiz.MyCourseId,
                        CourseTitle = myCourse.Course.Title,
                        Score = userQuiz.Score,
                        TotalQuestion = userQuiz.TotalQuestion,
                        SubmittedAt = userQuiz.CreatedAt,
                    };
                    quizzes.Add(quiz);
                }

                var totalTask = myCourse.TaskResults.Count;
                if (totalTask > 0)
                {
                    var completedTaskCount = myCourse.TaskResults.Count(x => x.IsCompleted);
                    var completedPercent = (int)Math.Round(completedTaskCount * 100.0 / totalTask, 2);

                    var task = new CourseTaskDashBoardDto()
                    {
                        MyCourseId = myCourse.Id,
                        CourseTitle = myCourse.Course.Title,
                        CompletedPercent = completedPercent,
                        SubmittedAt= myCourse.ModifiedAt,
                    };
                    tasks.Add(task);
                }
            }
            
            result.CountMyCourse = myCourses.Count;
            result.CountTaskSubmitted = countTaskResult;
            result.CountQuizSubmitted = countUserAttemp;
            result.CountFeedback = feedbacks.Count;
            result.Feedbacks = feedbacks.OrderByDescending(f => f.CreatedAt).ToList();
            result.Quizzes = quizzes.OrderByDescending(f => f.SubmittedAt).Take(50).ToList();
            result.Tasks = tasks.OrderByDescending(f => f.SubmittedAt).Take(50).ToList();

            return result;
        }

        public async Task<bool> UpdateLearnerProfile(LearnerProfileDto dto)
        {
            var learnerProfile = await _repository.GetSetAsTracking<LearnerProfileEntity>(p => p.Id == RuntimeContext.Current.UserId)
                .Include(p => p.Account)
                .FirstOrDefaultAsync() ?? throw new NotExistException("LearnerProfile");

            if (dto.MajorId != Guid.Empty)
            {
                var hasMajor = await _repository.AnyAsync<LearnerMajorEntity>(p => p.Id == dto.MajorId);
                if (!hasMajor)
                {
                    throw new DataValidationException("Major is not exist", "", CErrorCode.InvalidInput);
                }
            }

            learnerProfile.Account.Avatar = dto.Avatar;
            learnerProfile.Account.Name = dto.Name;
            learnerProfile.MajorId = dto.MajorId ?? Guid.Empty;

            await _repository.UpdateAsync(learnerProfile);

            return true;
        }

    }
}
