using Common.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.API
{
    [Route("api/v{version:apiVersion}/learner")]
    [ApiController]
    [APIObjectResult]
    [APIResultException]
    [Authorize]
    public class LearnerAPIBaseController
    {
        private readonly IHttpContextAccessor _accessor;
        protected HttpContext _httpContext
        {
            get { return _accessor.HttpContext; }
        }
        public LearnerAPIBaseController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
    }
}
