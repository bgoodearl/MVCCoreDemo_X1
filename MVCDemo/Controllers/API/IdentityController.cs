#if USE_IDSVR6
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCDemo.ComponentHelpers;
using MVCDemo.Models.UserInfo;
using System;

namespace MVCDemo.Controllers.API
{

    [Route("~/api/identity")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            TokenInfoResult result = new TokenInfoResult();
            try
            {
                if ((User != null) && (User.Identity != null) && User.Identity.IsAuthenticated)
                {
                    result.Username = User.Identity.Name;
                }
                User.GetClaimsInfoList(result.ClaimsInfo);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"{ex.GetType().Name}: {ex.Message}";
            }
            return Ok(result);
        }
    }
}
#endif
