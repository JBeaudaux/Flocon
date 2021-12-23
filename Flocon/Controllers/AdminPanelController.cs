using AspNetCore.Identity.Mongo.Model;
using Flocon.Mailing;
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
        private readonly IEmailSender _emailSender;
        private readonly ILogger<HomeController> _logger;

        public AdminPanelController(UserManager<UserFlocon> userManager,
                                    RoleManager<MongoRole> roleManager,
                                    CustomersService customersService,
                                    IEmailSender emailSender,
                                    ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _customersService = customersService;
            _emailSender = emailSender;
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

            // Create one super-user wuth contact mail
            var myUser = new UserFlocon { UserName = vm.NewCompany.CompanyName, Email = vm.NewCompany.ContactMail, CompanyId = vm.NewCompany.Id};
            var pwd = GetRandomPassword();
            var resCreate = _userManager.CreateAsync(myUser, pwd).Result;

            if (resCreate.Succeeded)
            {
                var resRole = _userManager.AddToRoleAsync(myUser, "Superuser").Result;
                _logger.LogInformation(String.Format("Super User created for company {0} :: UserId={1} :: CompanyId={2} :: RoleAssignation={3}",
                                       vm.NewCompany.CompanyName, myUser.Id, vm.NewCompany.Id, resRole.ToString()));

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(myUser);
                await _emailSender.SendRegisterEmailAsync(myUser.Email, "Admin", vm.NewCompany.CompanyName, pwd, "http://flocon.io");
                // ToDo : Send "real" confirmation link when merged with login
                // var callbackUrl = Url.EmailConfirmationLink(user.Id.ToString(), code, Request.Scheme);

            }
            else
            {
                _logger.LogInformation(String.Format("Failed registration for company {0} :: CompanyId={1}", vm.NewCompany.CompanyName, vm.NewCompany.Id));
            }

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

        [HttpGet]
        public async Task<IActionResult> RenewLicenceTrial(string id)
        {
            //vm.NewCompany.Id = "";
            var cmp = _customersService.GetCompany(id);
            cmp.LicenceExpiry = DateTime.Now.AddMonths(1);
            await _customersService.UpdateAsset(id, cmp);

            return RedirectToAction("CompanyProfile", "AdminPanel", new { id = id });
        }

        [HttpGet]
        public async Task<IActionResult> RenewLicenceYearly(string id)
        {
            //vm.NewCompany.Id = "";
            var cmp = _customersService.GetCompany(id);
            cmp.LicenceExpiry = DateTime.Now.AddMonths(12);
            await _customersService.UpdateAsset(id, cmp);

            return RedirectToAction("CompanyProfile", "AdminPanel", new { id = id });
        }

        // ToDo : Move to a "common" library
        /// <summary>
        /// Builds a random password for new users. Has 12 characters (3 upper case letters, 3 lower case letters, 3 digits, and 3 special characters)
        /// </summary>
        /// <param name="length">Length of the password</param>
        /// <returns></returns>
        public static string GetRandomPassword()
        {
            var length = 12;
            string validLettersUp = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            string validLettersDown = "abcdefghijklmnopqrstuvwxyz";
            string validDigits = "0123456789";
            string validSpecial = "!@#$%^&*?_-";

            Random random = new Random();

            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                if (i < 3)
                {
                    chars[i] = validLettersUp[random.Next(0, validLettersUp.Length)];
                }
                else if (i < 6)
                {
                    chars[i] = validLettersDown[random.Next(0, validLettersDown.Length)];
                }
                else if (i < 9)
                {
                    chars[i] = validDigits[random.Next(0, validDigits.Length)];
                }
                else
                {
                    chars[i] = validSpecial[random.Next(0, validSpecial.Length)];
                }
            }

            string pwdChars = new string(chars);
            string rand = new string(pwdChars.OrderBy(x => Guid.NewGuid()).ToArray());
            
            return rand;
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