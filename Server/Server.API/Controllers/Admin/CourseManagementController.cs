using Common.Domain;
using Microsoft.AspNetCore.Mvc;
using Server.API;
using Server.Domain.Admin;
using Server.Service.Admin;

namespace Server.Admin.API
{
    [ApiVersion("1")]
    public class CourseManagementController : APIBaseController
    {
        private readonly ICourseManagementService _courseManagementService;

        public CourseManagementController(IHttpContextAccessor accessor, ICourseManagementService courseManagementService) : base(accessor)
        {
            _courseManagementService = courseManagementService;
        }

        [HttpPost("course/paging")]
        public async Task<TableInfo<CourseDto>> GetPaging([FromBody] CTableParameter parameter)
        {
            return await _courseManagementService.GetPaging(parameter);
        }

        [HttpGet("course/{id}")]
        public async Task<CourseDetailDto> GetDetail(Guid id)
        {
            return await _courseManagementService.GetDetail(id);
        }

        [HttpPost("course")]
        public async Task<bool> Add([FromBody] CourseDetailDto dto)
        {
            return await _courseManagementService.Add(dto);
        }

        [HttpPut("course/{id}")]
        public async Task<bool> Update(Guid id, [FromBody] CourseDetailDto dto)
        {
            return await _courseManagementService.Update(id, dto);
        }

        [HttpPut("course/publish")]
        public async Task<bool> Publish(List<Guid> ids)
        {
            return await _courseManagementService.Publish(ids);
        }

        [HttpPut("course/unpublish")]
        public async Task<bool> Unpublish(List<Guid> ids)
        {
            return await _courseManagementService.Unpublish(ids);
        }
    }
}
