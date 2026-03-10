using Common.Domain;
using Common.Service;
using Server.Domain.Learner;

namespace Server.Service.Learner
{
    public interface ICourseService : IScopeDependency
    {
        Task<TableInfo<ViewCourseDto>> GetPaging(CTableParameter parameter, CourseType courseType);
        Task<MyCourseDto> GetDetail(Guid id);
        Task<bool> Apply(MyCourseCreateDto dto);
    }
}
