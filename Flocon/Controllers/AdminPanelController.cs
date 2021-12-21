using AspNetCore.Identity.Mongo.Model;
using Flocon.Models;
using Flocon.Models.AdminPanel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Flocon.Controllers
{
    // ToDo : Allow authenticated admins only
    public class AdminPanelController : Controller
    {
        private readonly UserManager<UserFlocon> _userManager;
        private readonly RoleManager<MongoRole> _roleManager;
        private readonly CustomersService _customersService;
        private readonly ILogger<HomeController> _logger;

        public AdminPanelController(UserManager<UserFlocon> userManager,
                                    RoleManager<MongoRole> roleManager,
                                    CustomersService customersService,
                                    ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _customersService = customersService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new IndexViewModel();
            vm.Users = _userManager.Users.ToList();
            vm.Roles = _roleManager.Roles.ToList();
            vm.Companies = _customersService.GetCompaniesList();

            vm.NewCompany = new Company();

            return View(vm);
        }

        //[HttpGet("/AdminPanel/CompanyProfile/{CmpyId}")]
        [HttpGet]

        public IActionResult CompanyProfile(string id)
        {
            var vm = new IndexViewModel();
            vm.Users = _userManager.Users.ToList();
            vm.Roles = _roleManager.Roles.ToList();
            vm.Companies = _customersService.GetCompaniesList();

            vm.NewCompany = _customersService.GetCompany(id);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewCompany(IndexViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.NewCompany.CompanyName) || string.IsNullOrEmpty(vm.NewCompany.ContactMail))
            {
                return RedirectToAction("Index", "AdminPanel");
            }

            var cmp = vm.NewCompany;
            cmp.MaxUsers = 0;
            cmp.Groups = new List<string>();
            cmp.CreatedOn = DateTime.Now;
            cmp.LicenceExpiry = DateTime.Now;
            await _customersService.CreateCompany(cmp);

            return RedirectToAction("CompanyProfile", "AdminPanel", new { id = cmp.Id });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCompany(string id)
        {
            await _customersService.DeleteCompany(id);
            return RedirectToAction("Index", "AdminPanel");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCompany(IndexViewModel vm)
        {
            string cmpId = vm.NewCompany.Id;
            //vm.NewCompany.Id = "";
            await _customersService.UpdateAsset(cmpId, vm.NewCompany);
            return RedirectToAction("CompanyProfile", "AdminPanel", new { id = cmpId });
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