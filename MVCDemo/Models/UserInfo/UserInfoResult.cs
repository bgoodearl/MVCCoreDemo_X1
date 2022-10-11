using System;
using SSC = System.Security.Claims;

namespace MVCDemo.Models.UserInfo
{
    internal class UserInfoResult
    {
        internal string CircuitId { get; set; }
        internal Exception Exception { get; set; }
        internal string LogErrorMessage { get; set; }
        internal SSC.ClaimsPrincipal User { get; set; }
        internal string UserIdentifier { get; set; }
        internal bool UserIsAuthenticated { get; set; }
        internal string UserName { get; set; } = string.Empty;
    }
}
