using Common.Domain;
using Common.Repository;
using Server.Domain.Admin;

namespace Server.Service.Admin
{
    public class AnswerManagementService(
        IDBRepository _repository
        ) : IAnswerManagementService
    {
        public async Task<bool> Add(AnswerDto dto)
        {
            var answer = new AnswerEntity
            {
                Id = Guid.NewGuid(),
                AnswerOrder = dto.AnswerOrder,
                Name = dto.Name,
                IsCorrect = dto.IsCorrect
            };

            await _repository.AddAsync(answer);
            return true;
        }

        public async Task<bool> Update(Guid id, AnswerDto dto)
        {
            var entity = await _repository.FindAsync<AnswerEntity>(p => p.Id == id) ?? throw new NotExistException("Major");
            entity.Name = dto.Name;
            entity.IsCorrect = dto.IsCorrect;
            entity.AnswerOrder = dto.AnswerOrder;

            await _repository.UpdateAsync(entity);

            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var entity = await _repository.FindAsync<AnswerEntity>(p => p.Id == id) ?? throw new NotExistException("Quiz");
            entity.IsDeleted = true;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
