using Microsoft.AspNetCore.Mvc;
using MvcJwtAuthApp.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace MvcJwtAuthApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.UserName = User.FindFirst("name")?.Value ?? "User";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Analytics()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.PageTitle = "Analytics";
            return View("Index"); // Reuses the same view with conditional rendering
        }

        public IActionResult Reports()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.PageTitle = "Reports";
            return View("Index"); // Reuses the same view with conditional rendering
        }

        public IActionResult Page1()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.PageTitle = "Dummy Page 1";
            return View("Index"); // Reuses the same view with conditional rendering
        }

        public IActionResult Page2()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.PageTitle = "Dummy Page 2";
            return View("Index"); // Reuses the same view with conditional rendering
        }
    }
}
