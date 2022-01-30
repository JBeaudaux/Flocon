using Flocon.Models.Home;
using Flocon.Services.FileManager;
using Flocon.Services.Mailing;
using Microsoft.AspNetCore.Mvc;

namespace Flocon.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailSender _emailSender;
        private IFileManager _fileManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IFileManager fileManager,
                              IEmailSender emailSender,
                              ILogger<HomeController> logger)
        {
            _fileManager = fileManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Error(string code)
        {
            //var err = new ErrorViewModel();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactUs(HomeViewModel vm)
        {
            // ToDo : SendGrid ContactUs
            await _emailSender.SendContactUsEmailAsync(vm.Email, vm.Name, vm.Subject, vm.Message);
            return RedirectToAction("Index");
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