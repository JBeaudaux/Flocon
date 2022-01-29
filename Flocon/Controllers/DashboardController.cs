using Microsoft.AspNetCore.Mvc;
using Flocon.Models.Dashboard;

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


        [HttpPost]
        public IActionResult UploadCraftDoc(DashViewModel vm)
        {
            
            return View("DraftDoc");
        }
    }
}