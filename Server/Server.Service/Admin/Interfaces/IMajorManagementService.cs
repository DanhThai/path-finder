using Common.Domain;
using Common.Service;
using Server.Domain.Admin;

namespace Server.Service.Admin
{
    public interface IMajorManagementService : IScopeDependency
    {
        Task<TableInfo<LearnerMajorDto>> GetPaging(CTableParameter parameter);
        Task<LearnerMajorDto> GetDetail(Guid id);
        Task<bool> Add(LearnerMajorDto dto);
        Task<bool> Update(Guid id, LearnerMajorDto dto);
        Task<bool> Delete(Guid id);

        Task<List<CComboxItem>> GetMajorSelectbox();
    }
}
