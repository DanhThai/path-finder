using Common.Domain;
using Microsoft.AspNetCore.Mvc;
using Server.Service.Admin;

namespace Server.API
{
    [ApiVersion("1")]
    public class ProfileController : APIBaseController
    {
        private readonly IAccountService _accountService;
        public ProfileController(IHttpContextAccessor accessor, IAccountService accountService) : base(accessor)
        {
            _accountService = accountService;
        }

        [HttpGet("me")]
        public async Task<AccountDto> Me()
        {
            return await _accountService.GetProfileAsync(_httpContext.User);
        }
    }
}
