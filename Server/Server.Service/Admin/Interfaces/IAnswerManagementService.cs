using Common.Service;
using Server.Domain.Admin;

namespace Server.Service.Admin
{
    public interface IAnswerManagementService : IScopeDependency
    {
        Task<bool> Add(AnswerDto dto);
        Task<bool> Update(Guid id, AnswerDto dto);
        Task<bool> Delete(Guid id);
    }
}
