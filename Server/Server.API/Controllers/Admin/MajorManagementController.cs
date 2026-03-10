using Common.Domain;
using Microsoft.AspNetCore.Mvc;
using Server.API;
using Server.Domain.Admin;
using Server.Service.Admin;
using Microsoft.AspNetCore.Authorization;

namespace Server.Admin.API
{
    [ApiVersion("1")]
    public class MajorManagementController : APIBaseController
    {
        private readonly IMajorManagementService _majorManagementService;
        public MajorManagementController(IHttpContextAccessor accessor, IMajorManagementService majorManagementService) : base(accessor)
        {
            _majorManagementService = majorManagementService;
        }

        [HttpPost("major/paging")]
        public async Task<TableInfo<LearnerMajorDto>> GetPaging([FromBody] CTableParameter parameter)
        {
            return await _majorManagementService.GetPaging(parameter);
        }

        [HttpGet("major/{id}")]
        public async Task<LearnerMajorDto> GetDetail(Guid id)
        {
            return await _majorManagementService.GetDetail(id);
        }

        [HttpPost("major")]
        public async Task<bool> Add([FromBody] LearnerMajorDto dto)
        {
            return await _majorManagementService.Add(dto);
        }

        [HttpPut("major/{id}")]
        public async Task<bool> Update(Guid id, [FromBody] LearnerMajorDto dto)
        {
            return await _majorManagementService.Update(id, dto);
        }

        [HttpDelete("major/{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _majorManagementService.Delete(id);
        }

        [HttpGet("major/selectbox")]
        [AllowAnonymous]
        public async Task<List<CComboxItem>> GetMajorSelectbox()
        {
            return await _majorManagementService.GetMajorSelectbox();
        }
    }
}
