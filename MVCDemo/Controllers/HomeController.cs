using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
#if USE_IDSVR6
using Microsoft.Extensions.Options;
using MVCDemo.Models.Configuration;
#endif
using MVCDemo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MVCDemo.Models.UserInfo;
using Newtonsoft.Json;
#if USE_IDSVR6
using System.Net.Http;
using System.Net.Http.Headers;
#endif
using System.Threading.Tasks;

namespace MVCDemo.Controllers
{
    [Authorize]
    [Route("~/x2/[Controller]/[Action]")]
    public class HomeController : Controller
    {
        #region Read Only variables
        private readonly ILogger<HomeController> _logger;
        #endregion Read Only variables

        public HomeController(
#if USE_IDSVR6
            IOptions<AppSettings> appSettings,
#endif
            ILogger<HomeController> logger)
        {
#if USE_IDSVR6
            AppSettings = appSettings.Value;
#endif
            _logger = logger;
        }

        #region Local Variables

#if USE_IDSVR6
        AppSettings AppSettings { get; }
#endif
        private int Index0Count { get; set; }

#endregion Local Variables

        [AllowAnonymous]
        [Route("~/")]
        public IActionResult Index0()
        {
            Index0Count++;
#if DEBUG
            if (Index0Count == 1) _logger.LogDebug("Home - Index0 count={0}", Index0Count);
#endif
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [Route("~/x2/")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

#if USE_IDSVR6
        public async Task<IActionResult> TokenTest()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if ((accessToken != null) && (AppSettings != null) && !string.IsNullOrWhiteSpace(AppSettings.apiUrlRoot))
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    string idSvrIdentityUrl = $"{AppSettings.apiUrlRoot}/identity";
                    var content = await client.GetStringAsync(idSvrIdentityUrl);

//#if DEBUG
//                    var parsed = System.Text.Json.JsonDocument.Parse(content);
//                    var formatted = System.Text.Json.JsonSerializer.Serialize(parsed, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
//#endif

                    TokenInfoResult tokenInfoResult = JsonConvert.DeserializeObject<TokenInfoResult>(content);
                    return View(tokenInfoResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TokenTest - {ex.GetType().Name}: {ex.Message}");
            }
            return RedirectToAction("Error");
        }
#endif

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
