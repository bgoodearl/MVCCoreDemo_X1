using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCDemo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        #region Local Variables

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

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
