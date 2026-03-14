using Common.Domain;
using Common.Service;
using Server.Domain.Admin;

namespace Server.Service.Admin
{
    public interface ICourseManagementService : IScopeDependency
    {
        Task<TableInfo<CourseDto>> GetPaging(CTableParameter parameter);
        Task<CourseDetailDto> GetDetail(Guid id);
        Task<bool> Add(CourseDetailDto dto);
        Task<bool> Update(Guid id, CourseDetailDto dto);
        Task<bool> Publish(List<Guid> ids);
        Task<bool> Unpublish(List<Guid> ids);
    }
}