using Common.Domain;
using Common.Service;
using Server.Domain.Admin;
using Server.Domain.Learner;

namespace Server.Service.Learner
{
    public interface ICourseService : IScopeDependency
    {
        Task<TableInfo<ViewCourseDto>> GetPaging(CTableParameter parameter, CourseType courseType);
        Task<CourseDetailDto> GetDetail(Guid id);
        Task<Guid> Apply(MyCourseCreateDto dto);
    }
}
