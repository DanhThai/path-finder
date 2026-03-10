using Common.Domain;
using Common.Service;

namespace Server.Service.Admin
{
    public interface IAccountManagementService : IScopeDependency
    {
        Task<TableInfo<AccountDto>> GetPaging(CTableParameter parameter);
        Task<AccountDto> GetDetail(Guid id);
        Task<bool> Add(SaveAccountDto dto);
        Task<bool> Update(Guid id, SaveAccountDto dto);
        Task<bool> Activate(Guid id);
        Task<bool> Deactivate(Guid id);
    }
}
