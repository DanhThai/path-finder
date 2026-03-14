
using Common.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Common.Service
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class APIResultExceptionAttribute : ExceptionFilterAttribute
    {
        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception is ErrorHandleException
                || context.Exception is NotExistException
                || context.Exception is WarningHandleException
                || context.Exception is DataValidationException)
            {
                //RuntimeContext.Logger.Warn($"Handled exception has been thrown in action {context.ActionDescriptor?.DisplayName}, Message {context.Exception?.Message}.");
            }
            else if (context.Exception is CustomExceptionBase
                || context.Exception is UnauthorizedAccessException)
            {
                //RuntimeContext.Logger.Warn($"Handled exception has been thrown in action {context.ActionDescriptor?.DisplayName}.", context.Exception);
            }
            else
            {
                var sb = new StringBuilder();
                //await AppendRequestDetails(context, sb);
                //RuntimeContext.Logger.Error($"An error occured while executing action {context.ActionDescriptor?.DisplayName}, request details: {sb}.", context.Exception);
            }

            if (context.Exception is UnauthorizedAccessException)
            {
                context.Result = new ObjectResult(
                   new CAPIResponseDto()
                   {
                       Code = StatusCodes.Status401Unauthorized,
                       Status = ResponseStatus.Unauthorized
                   });
                context.HttpContext.Response.StatusCode = 401;
            }
            else if (context.Exception is ErrorHandleException
                || context.Exception is NotExistException)
            {
                context.Result = new ObjectResult(
                   new CAPIResponseDto()
                   {
                       Code = StatusCodes.Status200OK,
                       Status = ResponseStatus.ModelInfoInvalid,
                       Message = context.Exception.Message,
                       ErrorCode = ((CustomExceptionBase)context.Exception).Code
                   });
            }
            else if (context.Exception is AccessDeniedException)
            {
                context.Result = new ObjectResult(
                    new CAPIResponseDto()
                    {
                        Code = StatusCodes.Status403Forbidden,
                        Status = ResponseStatus.AccessDenied,
                    });
            }
            else if (context.Exception is Error500Exception)
            {
                context.Result = new ObjectResult(
                   new CAPIResponseDto()
                   {
                       Code = StatusCodes.Status500InternalServerError,
                       Status = ResponseStatus.Error500Page
                   });
            }
            else if (context.Exception is Error404Exception)
            {
                context.Result = new ObjectResult(
                   new CAPIResponseDto()
                   {
                       Code = StatusCodes.Status404NotFound
                   });
            }
            else if (context.Exception is WarningHandleException)
            {
                context.Result = new ObjectResult(
                   new CAPIResponseDto()
                   {
                       Code = StatusCodes.Status200OK,
                       Status = ResponseStatus.WarningInfo,
                       Message = context.Exception.Message
                   });
            }
            else if (context.Exception is DataValidationException)
            {
                var exception = context.Exception as DataValidationException;
                context.Result = new ObjectResult(
                   new CAPIResponseValidationDto()
                   {
                       Code = StatusCodes.Status200OK,
                       Status = ResponseStatus.ValidationFailed,
                       Message = context.Exception.Message,
                       ValidateKey = exception.ValidateKey,
                       ErrorCode = ((CustomExceptionBase)context.Exception).Code
                   });
            }
            else
            {
                context.Result = new ObjectResult(
                   new CAPIResponseDto()
                   {
                       Code = StatusCodes.Status200OK,
                       Status = ResponseStatus.RequestError,
                       //system error should not throw to frontend.
                       Message = context.Exception?.Message
                   });
            }
        }
    }
}
