using Flocon.Services.FileManager;
using Microsoft.AspNetCore.Mvc;

namespace Flocon.Controllers
{
    public class HomeController : Controller
    {
        private IFileManager _fileManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IFileManager fileManager, 
                              ILogger<HomeController> logger)
        {
            _fileManager = fileManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        /*
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        */
    }
}