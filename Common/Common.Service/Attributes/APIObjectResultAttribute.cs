using Common.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Common.Service
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class APIObjectResultAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult)
            {
                if (context.Result is OkObjectResult)
                {
                    context.Result = new ObjectResult(new CAPIResponseDto
                    {
                        Code = StatusCodes.Status200OK,
                        Data = (context.Result as ObjectResult).Value
                    });
                }
                else if (context.Result is BadRequestObjectResult)
                {
                    //RuntimeContext.Logger.Debug($"BadRequestObjectResult, {(context.Result as BadRequestObjectResult)?.Value}");
                    context.Result = new ObjectResult(new CAPIResponseDto
                    {
                        Code = StatusCodes.Status400BadRequest,
                        //Data = (context.Result as BadRequestObjectResult).Value
                    });
                }
                else if (context.Result is UnauthorizedObjectResult)
                {
                    context.Result = new ObjectResult(new CAPIResponseDto
                    {
                        Code = StatusCodes.Status401Unauthorized,
                        Status = ResponseStatus.Unauthorized
                    });
                }
                else if (context.Result is NotFoundObjectResult || context.Result is NotFoundResult)
                {
                    context.Result = new ObjectResult(new CAPIResponseDto
                    {
                        Code = StatusCodes.Status404NotFound,
                    });
                }
                else if (context.Result is CreatedAtActionResult || context.Result is CreatedAtRouteResult || context.Result is CreatedResult)
                {
                    context.Result = new ObjectResult(new CAPIResponseDto
                    {
                        Code = StatusCodes.Status201Created,
                    });
                }
                else
                {
                    var objR = context.Result as ObjectResult;
                    context.Result = new ObjectResult(new CAPIResponseDto
                    {
                        Code = objR.StatusCode.HasValue ? objR.StatusCode.Value : StatusCodes.Status200OK,
                        Data = objR.Value
                    });
                }
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new CAPIResponseDto
                {
                    Code = StatusCodes.Status404NotFound,
                });
            }
            else if (context.Result is ContentResult)
            {
                context.Result = new ObjectResult(new CAPIResponseDto
                {
                    Code = StatusCodes.Status200OK,
                    Data = (context.Result as ContentResult).Content
                });
            }
            else if (context.Result is StatusCodeResult)
            {
                context.Result = new ObjectResult(new CAPIResponseDto
                {
                    Code = (context.Result as StatusCodeResult).StatusCode,
                });
            }
            else if (context.Result is FileResult)
            {
                // add headers so that front can read file name.
                string fileName = ((FileResult)context.Result).FileDownloadName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    //if (!context.HttpContext.Response.Headers.ContainsKey(CoreConstants.AccessControlExposeHeaderName))
                    //{
                    //    context.HttpContext.Response.Headers.Add(CoreConstants.AccessControlExposeHeaderName, CoreConstants.ContentDispositionHeader);
                    //}
                    //else
                    //{
                    //    var names = context.HttpContext.Response.Headers[CoreConstants.AccessControlExposeHeaderName];
                    //    var array = names.ToString().Split(',');
                    //    if (!array.Any(s => s.Trim() == CoreConstants.ContentDispositionHeader))
                    //    {
                    //        string value = names.ToString().Trim();
                    //        context.HttpContext.Response.Headers[CoreConstants.AccessControlExposeHeaderName] = $"{value}{((value.EndsWith(",") || string.IsNullOrEmpty(value)) ? "" : ",")}{CoreConstants.ContentDispositionHeader}";
                    //    }
                    //}
                }
            }
            else if (context.Result is RedirectResult)
            {
                //Do nothing
            }
            else
            {
                var resultObj = context.Result as ObjectResult;
                context.Result = new ObjectResult(new CAPIResponseDto
                {
                    Code = resultObj.StatusCode.HasValue ? resultObj.StatusCode.Value : StatusCodes.Status200OK,
                    Data = resultObj.Value
                });
            }
        }
    }
}
