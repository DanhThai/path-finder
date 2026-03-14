using Common.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Domain.Admin;
using Server.Service.Admin;

namespace Server.API.Controllers.Admin
{
    [ApiVersion("1")]
    public class CourseCategoryController : APIBaseController
    {
        private readonly ICourseCategoryService _courseCategoryService;
        public CourseCategoryController(IHttpContextAccessor accessor, ICourseCategoryService courseCategoryService) : base(accessor)
        {
            _courseCategoryService = courseCategoryService;
        }

        [HttpPost("coursecategory/paging")]
        public async Task<TableInfo<CourseCategoryDto>> GetPaging([FromBody] CTableParameter parameter)
        {
            return await _courseCategoryService.GetPaging(parameter);
        }

        [HttpGet("coursecategory/{id}")]
        public async Task<CourseCategoryDto> GetDetail(Guid id)
        {
            return await _courseCategoryService.GetDetail(id);
        }

        [HttpPost("coursecategory")]
        public async Task<bool> Add([FromBody] CourseCategoryDto dto)
        {
            return await _courseCategoryService.Add(dto);
        }

        [HttpPut("coursecategory/{id}")]
        public async Task<bool> Update(Guid id, [FromBody] CourseCategoryDto dto)
        {
            return await _courseCategoryService.Update(id, dto);
        }

        [HttpDelete("coursecategory/{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _courseCategoryService.Delete(id);
        }

        [HttpGet("coursecategory/selectbox")]
        [AllowAnonymous]
        public async Task<List<CComboxItem>> GetCategorySelectbox()
        {
            return await _courseCategoryService.GetCategorySelectbox();
        }
    }
}
