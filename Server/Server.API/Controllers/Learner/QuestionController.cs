using Microsoft.AspNetCore.Mvc;
using Server.API;
using Server.Domain.Admin;
using Server.Service.Learner;

namespace Server.Learner.API
{
    [ApiVersion("1")]
    public class QuestionController : APIBaseController
    {
        private readonly IQuestionService _questionService;
        public QuestionController(IHttpContextAccessor accessor, IQuestionService questionService) : base(accessor)
        {
            _questionService = questionService;
        }

        [HttpGet("question/courseId={courseId}")]
        public async Task<List<QuestionDto>> GetQuestionByCourseId([FromRoute] Guid courseId)
        {
            return await _questionService.GetQuestionByCourseId(courseId);
        }
    }
}
