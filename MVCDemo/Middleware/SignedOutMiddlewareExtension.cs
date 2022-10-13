using Microsoft.AspNetCore.Builder;

namespace MVCDemo.Middleware
{
#if !USE_IDSVR6
    public static class SignedOutMiddlewareExtension
    {
        public static IApplicationBuilder UseSignedOutMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SignedOutMiddleware>();
        }
    }
#endif
}
