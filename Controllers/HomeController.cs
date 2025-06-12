using Microsoft.AspNetCore.Mvc;
using NailMVC.Models;
using System.Diagnostics;

namespace NailMVC.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Ping()
        {
            var user = HttpContext.Session.GetString("UserEmail");
            return Ok(); // Không cần làm gì, chỉ cần chạm session là nó "sống"
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
