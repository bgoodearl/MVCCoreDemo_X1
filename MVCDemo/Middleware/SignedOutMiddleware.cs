using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MVCDemo.Models;
using MVCDemo.Models.Configuration;
using System;
using System.Threading.Tasks;

namespace MVCDemo.Middleware
{
#if !USE_IDSVR6
    public class SignedOutMiddleware
    {
        private readonly AppSettings _appSettings;
        private readonly RequestDelegate _next;

        public SignedOutMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if ((context != null) && (context.Request != null) && !string.IsNullOrWhiteSpace(context.Request.Path))
            {
                try
                {
                    PathString path = context.Request.Path;
                    if (path == "/signout-callback-oidc")
                    {
                        string authCookieName = ((_appSettings != null) && !string.IsNullOrWhiteSpace(_appSettings.authCookieName)) ? _appSettings.authCookieName
                            : MVCDemoDefs.Defaults.authCookieName;
                        if (context.Request.Cookies.ContainsKey(authCookieName))
                        {
                            context.Response.Cookies.Append(authCookieName, ""); //***TODO: Better way to get rid of cookie?
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    int foo = 1; //***TODO: Log this
#endif
                }
            }
            await _next(context);
        }
    }
#endif
}
