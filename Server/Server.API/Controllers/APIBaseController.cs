using Common.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.API
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [APIObjectResult]
    [APIResultException]
    [Authorize]
    public class APIBaseController
    {
        private readonly IHttpContextAccessor _accessor;
        protected HttpContext _httpContext
        {
            get { return _accessor.HttpContext; }
        }
        public APIBaseController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
    }
}
