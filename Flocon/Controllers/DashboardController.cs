using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Flocon.Models.Dashboard;
using Flocon.Services.FileManager;
using Flocon.Models;
using AspNetCore.Identity.Mongo.Model;

namespace Flocon.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<UserFlocon> _userManager;
        private readonly RoleManager<MongoRole> _roleManager;
        private readonly CustomersService _customersService;
        private IFileManager _fileManager;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(UserManager<UserFlocon> userManager,
                                   RoleManager<MongoRole> roleManager,
                                   CustomersService customersService,
                                   IFileManager fileManager,
                                   ILogger<DashboardController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _customersService = customersService;
            _fileManager = fileManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UploadCraftDoc(DashViewModel vm)
        {
            if (vm.DocToSign == null)
            {
                return View("Index");
            }

            var filePath = await _fileManager.SaveDocument(vm.DocToSign);
            vm.SignDoc = new SignTrail();
            vm.SignDoc.FileName = vm.DocToSign.FileName;
            vm.SignDoc.LocalStorageName = filePath;

            // Get file hash
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = vm.DocToSign.OpenReadStream())
                {
                    var hash = md5.ComputeHash(stream);
                    var unifHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    vm.SignDoc.FileHash = unifHash;
                }
            }

            // Prepare PDF preview
            vm.PdfPath = $"/signsdocs/{filePath}#toolbar=0";

            return View("DraftDoc", vm);
        }

        [HttpPost]
        public IActionResult CreateSignTrail(DashViewModel vm)
        {
            if (vm.SignDoc.UploadOriginalIPFS == true)
            {
                // Upload document on IPFS
            }
            return View("DraftDoc", vm);
        }
    }
}