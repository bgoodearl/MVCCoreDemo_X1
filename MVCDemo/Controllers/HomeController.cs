#if USE_IDSVR6
using Microsoft.AspNetCore.Authentication;
#endif
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MVCDemo.Models;
using MVCDemo.Models.Configuration;
using System;
//using System.Collections.Generic;
using System.Diagnostics;
//using System.Linq;
using System.Net.Http;
#if USE_IDSVR6
using System.Net.Http.Headers;
#endif
using MVCDemo.Models.UserInfo;
using Newtonsoft.Json;
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
            IOptions<AppSettings> appSettings,
            ILogger<HomeController> logger)
        {
            AppSettings = appSettings.Value;
            _logger = logger;
        }

#region Local Variables

        AppSettings AppSettings { get; }
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

#if true //USE_IDSVR6
        public async Task<IActionResult> TokenTest()
        {
            try
            {
                if ((AppSettings != null) && !string.IsNullOrWhiteSpace(AppSettings.apiUrlRoot))
                {
                    var client = new HttpClient();
                    string identityApiUrl = $"{AppSettings.apiUrlRoot}/identity";
#if USE_IDSVR6
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    if (accessToken != null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    }
                    else
                    {
                        TokenInfoResult tokenInfoResult = new TokenInfoResult
                        {
                            ErrorMessage = "No Access Token"
                        };
                    }
                    if (accessToken != null)
#endif
                    {
                        var content = await client.GetStringAsync(identityApiUrl);
//#if DEBUG
//                        var parsed = System.Text.Json.JsonDocument.Parse(content);
//                        var formatted = System.Text.Json.JsonSerializer.Serialize(parsed, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
//#endif
                        TokenInfoResult tokenInfoResult = null;
                        try
                        {
                            tokenInfoResult = JsonConvert.DeserializeObject<TokenInfoResult>(content);
                        }
                        catch (JsonReaderException ex)
                        {
                            _logger.LogError($"TokenTest - {ex.GetType().Name}: {ex.Message}");
                            //_logger.LogDebug(content);
                            tokenInfoResult = new TokenInfoResult
                            {
                                ErrorMessage = $"{ex.GetType().Name}: {ex.Message}"
                            };
                        }
                        if (tokenInfoResult != null)
                        {
                            return View(tokenInfoResult);
                        }
                    }
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
