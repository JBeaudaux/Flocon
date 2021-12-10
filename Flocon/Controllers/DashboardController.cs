using Flocon.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Flocon.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public DashboardController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}