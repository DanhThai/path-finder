using Common.Domain;
using Common.Repository;
using Server.Domain.Admin;

namespace Server.Service.Admin
{
    public class QuestionManagementService(
        IDBRepository _repository
        ) : IQuestionManagementService
    {
        public async Task<bool> Add(QuestionDto dto)
        {
            var question = new QuestionEntity
            {
                Id = Guid.NewGuid(),
                QuestionOrder = dto.QuestionOrder,
                Name = dto.Name,

                Answers = dto.Answers.Select(a => new AnswerProperty
                {
                    Id = Guid.NewGuid(),
                    Order = a.Order,
                    Name = a.Name,
                    IsCorrect = a.IsCorrect
                }).ToList()
            };

            await _repository.AddAsync(question);
            return true;
        }

        public async Task<bool> Update(Guid id, QuestionDto dto)
        {
            var entity = await _repository.FindAsync<QuestionEntity>(p => p.Id == id) ?? throw new NotExistException("Major");
            entity.Name = dto.Name;

            await _repository.UpdateAsync(entity);

            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var entity = await _repository.FindAsync<QuestionEntity>(p => p.Id == id) ?? throw new NotExistException("Quiz");
            entity.IsDeleted = true;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
