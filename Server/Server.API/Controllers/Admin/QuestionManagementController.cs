using Common.Domain;
using Microsoft.AspNetCore.Mvc;
using Server.API;
using Server.Domain.Admin;
using Server.Service.Admin;

namespace Server.Admin.API
{
    [ApiVersion("1")]
    public class QuestionManagementController : APIBaseController
    {
        private readonly IQuestionManagementService _questionManagementService;
        public QuestionManagementController(IHttpContextAccessor accessor, IQuestionManagementService questionManagementService) : base(accessor)
        {
            _questionManagementService = questionManagementService;
        }

        [HttpPost("question")]
        public async Task<bool> Add([FromBody] QuestionDto dto)
        {
            return await _questionManagementService.Add(dto);
        }

        [HttpPut("question/{id}")]
        public async Task<bool> Update(Guid id, [FromBody] QuestionDto dto)
        {
            return await _questionManagementService.Update(id, dto);
        }

        [HttpDelete("question/{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _questionManagementService.Delete(id);
        }
    }
}
