using Common.Domain;
using Microsoft.AspNetCore.Mvc;
using Server.API;
using Server.Domain.Admin;
using Server.Service.Admin;

namespace Server.Admin.API
{
    [ApiVersion("1")]
    public class AnswerManagementController : APIBaseController
    {
        private readonly IAnswerManagementService _answerManagementService;
        public AnswerManagementController(IHttpContextAccessor accessor, IAnswerManagementService answerManagementService) : base(accessor)
        {
            _answerManagementService = answerManagementService;
        }

        [HttpPost("answer")]
        public async Task<bool> Add([FromBody] AnswerDto dto)
        {
            return await _answerManagementService.Add(dto);
        }

        [HttpPut("answer/{id}")]
        public async Task<bool> Update(Guid id, [FromBody] AnswerDto dto)
        {
            return await _answerManagementService.Update(id, dto);
        }

        [HttpDelete("answer/{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _answerManagementService.Delete(id);
        }
    }
}
