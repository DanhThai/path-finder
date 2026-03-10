using CloudinaryDotNet;
using Common.Domain;
using Common.Repository;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Server.Domain.Learner;
using System.Data.Common;

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
                })
                .FirstOrDefaultAsync() ?? throw new NotExistException("LearnerProfile");

            var major = await _repository.GetSet<LearnerMajorEntity>(p => p.Id == learner.MajorId).FirstOrDefaultAsync();
            learner.MajorName = major?.Name;

            var count = await _repository.ExecuteScalar(
                        "SELECT  FROM {TableName.MyCourse} WHERE user_id = @userId",
                        new List<DbParameter> { new NpgsqlParameter("@userId", userId) }
                    );

            return learner;
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

            learnerProfile.Account.Name = dto.Name;
            learnerProfile.MajorId = dto.MajorId;

            await _repository.UpdateAsync(learnerProfile);

            return true;
        }

    }
}
