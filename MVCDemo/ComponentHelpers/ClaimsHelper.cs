using MVCDemo.Models.UserInfo;
using System;
using System.Collections.Generic;
using SSC = System.Security.Claims;

namespace MVCDemo.ComponentHelpers
{
    public static class ClaimsHelper
    {
        public static void GetClaimsInfoList(this SSC.ClaimsPrincipal principal, List<ClaimInfo> claimsInfoList)
        {
            if ((principal != null) && (principal.Identity != null) && principal.Identity.IsAuthenticated
                && (principal.Claims != null))
            {
                if (claimsInfoList != null)
                {
                    foreach (var claim in principal.Claims)
                    {
                        string claimValue = claim.Value;
                        if (!string.IsNullOrWhiteSpace(claim.ValueType)
                            && ((claim.ValueType == SSC.ClaimValueTypes.Integer)
                                || (claim.ValueType == "JSON")))
                        {
                            long unixDateTime;
                            if (!string.IsNullOrWhiteSpace(claimValue) && long.TryParse(claimValue, out unixDateTime))
                            {
                                try
                                {
                                    DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unixDateTime);
                                    claimValue = $"{claimValue} -- {dto:yyyy-MM-dd h:mm:ss tt} (UTC)";
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    int foo = 1;
#endif
                                }
                            }
                        }
                        claimsInfoList.Add(new ClaimInfo
                        {
                            ClaimType = claim.Type,
                            ClaimValue = claimValue,
                            ClaimValueType = claim.ValueType
                        });
                    }
                }
            }
        }

        public static List<ClaimInfo> GetClaimsInfoList(this SSC.ClaimsPrincipal principal)
        {
            List<ClaimInfo> claimsInfoList = new List<ClaimInfo>();
            principal.GetClaimsInfoList(claimsInfoList);

            return claimsInfoList;
        }
    }
}
