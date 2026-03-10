using Common.Domain;
using Common.Service;
using Server.Domain.Admin;

namespace Server.Service.Admin
{
    public interface ICourseCategoryService : IScopeDependency
    {
        Task<TableInfo<CourseCategoryDto>> GetPaging(CTableParameter parameter);
        Task<CourseCategoryDto> GetDetail(Guid id);
        Task<bool> Add(CourseCategoryDto dto);
        Task<bool> Update(Guid id, CourseCategoryDto dto);
        Task<bool> Delete(Guid id);

        Task<List<CComboxItem>> GetCategorySelectbox();
    }
}
