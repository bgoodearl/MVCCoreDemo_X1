using Microsoft.AspNetCore.Mvc;
using MVCDemo.Models.Demo;

namespace MVCDemo.Controllers
{
    [Route("~/x2/[Controller]/[Action]")]
    public class DemoController : Controller
    {
        [Route("~/x2/[Controller]")]
        public IActionResult Index()
        {
            DemoIndexViewModel model = new DemoIndexViewModel();
            return View(model);
        }

        public IActionResult Counter()
        {
            return View();
        }

        public IActionResult Forecast()
        {
            return View();
        }

        public IActionResult UserInfo()
        {
            return View();
        }
    }
}
