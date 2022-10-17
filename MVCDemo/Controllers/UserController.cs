//#define USE_IDSVR6
#if USE_IDSVR6
#else
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
#endif
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVCDemo.Controllers
{
    [Route("[Controller]/[Action]")]
    public class UserController : Controller
    {
        [AllowAnonymous]
        [ResponseCache(Duration = 0, NoStore = true)]
        [Route("~/x2/signin")]
        public IActionResult SignIn()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
#if USE_IDSVR6
                IActionResult actionResult = Challenge("oidc");
                return actionResult;
#else
                return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
#endif
            }
            return RedirectToAction("Index", "Home", null);
        }


        [Authorize]
        [Route("~/x2/signout")]
        public IActionResult SignOut()
        {
#if USE_IDSVR6
            return SignOut("Cookies", "oidc");
#else
            var signOutResult = SignOut(OpenIdConnectDefaults.AuthenticationScheme);
            return signOutResult;
#endif
        }

        [AllowAnonymous]
        [Route("~/x2/signedout")]
        public IActionResult SignedOut()
        {
            return RedirectToAction("Index", "Home", null);
        }

    }
}
