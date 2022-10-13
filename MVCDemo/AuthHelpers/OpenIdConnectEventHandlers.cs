using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MVCDemo.Models.LocalDefs;
using SSC = System.Security.Claims;

namespace MVCDemo.AuthHelpers
{
    internal static class OpenIdConnectEventHandlers
    {
        internal static Task OnTokenValidatedFunc(TokenValidatedContext context)
        {
            //query the database to get the role

            // add claims
            var claims = new List<SSC.Claim>
                            {
                                new SSC.Claim(SSC.ClaimTypes.Role, "Tester")
                            };
            DateTimeOffset dtoNow = new DateTimeOffset(DateTime.UtcNow);
            claims.Add(new SSC.Claim(LocalClaimTypes.LatestAuthenticationTime, dtoNow.ToUnixTimeSeconds().ToString(), SSC.ClaimValueTypes.Integer));

            var appIdentity = new SSC.ClaimsIdentity(claims);

            context.Principal.AddIdentity(appIdentity);

            return Task.CompletedTask;
        }
    }
}
