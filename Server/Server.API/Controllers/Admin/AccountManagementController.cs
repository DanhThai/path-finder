using Common.Domain;
using Microsoft.AspNetCore.Mvc;
using Server.API;
using Server.Service.Admin;

namespace Server.Admin.API
{
    [ApiVersion("1")]
    public class AccountManagementController : APIBaseController
    {
        private readonly IAccountManagementService _accountManagementService;
        public AccountManagementController(IHttpContextAccessor accessor, IAccountManagementService accountManagementService) : base(accessor)
        {
            _accountManagementService = accountManagementService;
        }

        [HttpPost("account/paging")]
        public async Task<TableInfo<AccountDto>> GetPaging([FromBody] CTableParameter parameter)
        {
            return await _accountManagementService.GetPaging(parameter);
        }

        [HttpGet("account/{id}")]
        public async Task<AccountDto> GetDetail(Guid id)
        {
            return await _accountManagementService.GetDetail(id);
        }

        [HttpPost("account")]
        public async Task<bool> Add([FromBody] SaveAccountDto dto)
        {
            return await _accountManagementService.Add(dto);
        }

        [HttpPut("account/{id}")]
        public async Task<bool> Update(Guid id, [FromBody] SaveAccountDto dto)
        {
            return await _accountManagementService.Update(id, dto);
        }

        [HttpPut("account/activate/{id}")]
        public async Task<bool> Activate(Guid id)
        {
            return await _accountManagementService.Activate(id);
        }

        [HttpPut("account/deactivate/{id}")]
        public async Task<bool> Deactivate(Guid id)
        {
            return await _accountManagementService.Deactivate(id);
        }
    }
}
