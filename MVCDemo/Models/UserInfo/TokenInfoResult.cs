using System.Collections.Generic;

namespace MVCDemo.Models.UserInfo
{
    public class TokenInfoResult
    {
        public TokenInfoResult()
        {
            ClaimsInfo = new List<ClaimInfo>();
        }

        public List<ClaimInfo> ClaimsInfo { get; protected set; }
        public string ErrorMessage { get; set; }
        public string Username { get; set; }
    }
}
