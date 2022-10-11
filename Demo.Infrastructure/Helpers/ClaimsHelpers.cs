using System.Linq;
using SSC = System.Security.Claims;
using SSP = System.Security.Principal;

namespace Demo.Infrastructure.Helpers
{
    internal static class ClaimsHelpers
    {
        public static SSC.Claim GetClaim(this SSC.ClaimsPrincipal principal, string claimType)
        {
            if ((principal != null) && (principal.Claims != null) && !string.IsNullOrWhiteSpace(claimType))
            {
                foreach (var claim in principal.Claims)
                {
                    if (claim.Type == claimType)
                        return claim;
                }
            }
            return null;
        }

        public static string GetName(this SSP.IPrincipal principal)
        {
            if ((principal != null) && (principal.Identity != null) && principal.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(principal.Identity.Name))
                {
                    return principal.Identity.Name;
                }
                else
                {
                    SSC.ClaimsIdentity claimsIdentity = principal.Identity as SSC.ClaimsIdentity;
                    if ((claimsIdentity != null) && (claimsIdentity.Claims != null))
                    {
                        SSC.Claim nameClaim = claimsIdentity.Claims
                            .Where(c => !string.IsNullOrWhiteSpace(c.Type) && c.Type == SSC.ClaimTypes.Name)
                            .FirstOrDefault();
                        if (nameClaim != null)
                        {
                            return nameClaim.Value;
                        }
                    }
                }
            }
            return null;
        }
    }
}
