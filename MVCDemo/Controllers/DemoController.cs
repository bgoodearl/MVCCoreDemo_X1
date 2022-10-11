﻿using Microsoft.AspNetCore.Mvc;

namespace MVCDemo.Controllers
{
    [Route("[Controller]/[Action]")]
    public class DemoController : Controller
    {
        [Route("~/[Controller]")]
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
