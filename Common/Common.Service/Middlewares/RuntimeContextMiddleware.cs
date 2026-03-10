using Common.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Common.Service
{
    public class RuntimeContextMiddleware
    {
        private readonly RequestDelegate _next;

        public RuntimeContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var wrapper = new RuntimeContextWrapper();
            try
            {
                var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
                if (endpoint != null)
                {
                    var authorizeAttr = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
                    if (authorizeAttr != null)
                    {
                    }
                }
                await wrapper.Init(context);

                if (wrapper.ErrorCode > 0)
                {
                    //RuntimeContext.Logger.Info(wrapper.ErrorMsg);
                    var r = new ObjectResult(
                        new CAPIResponseDto()
                        {
                            Code = wrapper.ErrorCode,
                            Status = ResponseStatus.WarningInfo
                        });
                    context.Response.StatusCode = wrapper.ErrorCode;
                    await context.Response.WriteAsJsonAsync(r);
                    return;
                }
                try

                {
                    var user = context.User;
                    if (Guid.TryParse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
                    {
                        RuntimeContext.Current.UserId = userId;
                        RuntimeContext.Current.User = new CUser()
                        {
                            Email = user.FindFirst(ClaimTypes.Email)?.Value,
                            Username = user.FindFirst(ClaimTypes.Name)?.Value,
                        };
                    }
                    await _next(context);
                }
                catch (Exception ex)
                {
                    //RuntimeContext.Logger.Warn($"Failed to login. User Id:{userId?.ToString() ?? ""}. User Name:{userName?.ToString() ?? ""}. Path: {context.Request.Path}. Exception: {ex}.");

                    string path = context.Request.Path.ToString();
                    if (path.EndsWith(""))
                    {
                        await context.SignOutAsync(IdentityConstants.ApplicationScheme);
                        context.Response.Redirect("/");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            finally
            {
                wrapper.Dispose();
            }
        }
    }

    public class RuntimeContextWrapper
    {
        public int ErrorCode { get; private set; } = 0;
        public string ErrorMsg { get; private set; }
        public RuntimeContextWrapper()
        {
            RuntimeContext.Current = new RuntimeContextInstance();
        }

        public async Task Init(HttpContext context)
        {
            if (context != null)
            {
                //this.AddHeaders(context);

                if (string.IsNullOrWhiteSpace(RuntimeContext.Current.ClientIP))
                {
                    RuntimeContext.Current.ClientIP = GetClientIP(context);
                }
            }
        }

        private string GetClientIP(HttpContext context)
        {
            string ip = string.Empty;//GetHeaderValueAs(context, "X-Real-Ip");

            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = GetHeaderValueAs(context, "X-Forwarded-For")?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }

            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (string.IsNullOrWhiteSpace(ip) && context.Connection?.RemoteIpAddress != null)
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = GetHeaderValueAs(context, "REMOTE_ADDR");
            }

            if (!string.IsNullOrWhiteSpace(ip) && ip.IndexOf(":") > 0)
            {
                ip = ip.Substring(0, ip.IndexOf(":"));
            }

            return ip?.Trim();
        }

        private string GetHeaderValueAs(HttpContext context, string headerName)
        {
            StringValues values;

            if (context.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                string rawValues = values.ToString();

                if (!string.IsNullOrWhiteSpace(rawValues))
                    return rawValues;
            }
            return string.Empty;
        }

        public void Dispose()
        {
            RuntimeContext.Current = null;
        }
    }
}
