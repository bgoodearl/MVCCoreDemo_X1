using Microsoft.AspNetCore.Mvc;

namespace MVCDemo.Controllers
{
    [Route("~/x2/[Controller]/[Action]")]
    public class DemoController : Controller
    {
        [Route("~/x2/[Controller]")]
        public IActionResult Index()
        {
            return View();
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
