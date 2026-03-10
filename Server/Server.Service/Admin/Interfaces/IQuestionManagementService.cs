using Common.Domain;
using Common.Service;
using Server.Domain.Admin;

namespace Server.Service.Admin
{
    public interface IQuestionManagementService : IScopeDependency
    {
        Task<bool> Add(QuestionDto dto);
        Task<bool> Update(Guid id, QuestionDto dto);
        Task<bool> Delete(Guid id);
    }
}
