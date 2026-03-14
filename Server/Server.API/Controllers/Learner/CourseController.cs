using Common.Domain;
using Microsoft.AspNetCore.Mvc;
using Server.API;
using Server.Domain.Admin;
using Server.Domain.Learner;
using Server.Service.Learner;

namespace Server.Learner.API
{
    [ApiVersion("1")]
    public class CourseController : LearnerAPIBaseController
    {
        private readonly ICourseService _courseService;
        public CourseController(IHttpContextAccessor accessor, ICourseService courseService) : base(accessor)
        {
            _courseService = courseService;
        }

        [HttpPost("course/simulation/paging")]
        public async Task<TableInfo<ViewCourseDto>> GetsimulationPaging([FromBody] CTableParameter parameter)
        {
            return await _courseService.GetPaging(parameter, CourseType.SimulationCourse);
        }

        [HttpPost("course/careervideo/paging")]
        public async Task<TableInfo<ViewCourseDto>> GetCareerVideoPaging([FromBody] CTableParameter parameter)
        {
            return await _courseService.GetPaging(parameter, CourseType.CareerVideo);
        }

        [HttpGet("course/{id}")]
        public async Task<CourseDetailDto> GetDetail(Guid id)
        {
            return await _courseService.GetDetail(id);
        }

        [HttpPost("course/apply")]
        public async Task<Guid> Apply([FromBody] MyCourseCreateDto dto)
        {
            return await _courseService.Apply(dto);
        }
    }
}
