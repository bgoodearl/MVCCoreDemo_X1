using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;

namespace MVCDemo.AuthHelpers
{
    public class JwtBearerEventHandlers
    {
        internal static Task OnAuthenticationFailedHandler(AuthenticationFailedContext context)
        {

            return Task.CompletedTask;
        }
    }
}
