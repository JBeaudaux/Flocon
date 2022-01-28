using AspNetCore.Identity.Mongo.Model;
using Flocon.Services.Mailing;
using Flocon.Models;
using Flocon.Models.AdminPanel;
using Flocon.Services.FileManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flocon.Controllers
{
    // ToDo : Allow authenticated admins only
    public class AdminPanelController : Controller
    {
        private readonly UserManager<UserFlocon> _userManager;
        private readonly RoleManager<MongoRole> _roleManager;
        private readonly CustomersService _customersService;
        private readonly IEmailSender _emailSender;
        private IFileManager _fileManager;
        private readonly ILogger<HomeController> _logger;

        public AdminPanelController(UserManager<UserFlocon> userManager,
                                    RoleManager<MongoRole> roleManager,
                                    CustomersService customersService,
                                    IEmailSender emailSender,
                                    IFileManager fileManager,
                                    ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _customersService = customersService;
            _emailSender = emailSender;
            _fileManager = fileManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new IndexViewModel();
            vm.Users = _userManager.Users.ToList();
            vm.Roles = _roleManager.Roles.ToList();
            vm.Companies = _customersService.GetCompaniesList();

            vm.NewCompany = new Company();

            var connectedUser = await GetConnectedUser();
            

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
            vm.NewUsrPass = GetRandomPassword();

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
                _logger.LogInformation(String.Format("Failed registration for company {0} :: CompanyId={1}",
                                                      vm.NewCompany.CompanyName, vm.NewCompany.Id));
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
            var company = _customersService.GetCompany(cmpId);

            // Trick : Copy to ensure that field not in the form are not erased.
            company.CompanyName = vm.NewCompany.CompanyName;
            company.BusinessField = vm.NewCompany.BusinessField;
            company.Description = vm.NewCompany.Description;
            company.ContactName = vm.NewCompany.ContactName;
            company.ContactAddress = vm.NewCompany.ContactAddress;
            company.ContactPhone = vm.NewCompany.ContactPhone;
            company.ContactMail = vm.NewCompany.ContactMail;
            company.ContactWebpage = vm.NewCompany.ContactWebpage;
            company.MaxUsers = vm.NewCompany.MaxUsers;

            if (vm.NewAvatar != null)
            {
                company.LogoPath = await _fileManager.SaveImage(vm.NewAvatar);
            }

            //vm.NewCompany.Id = "";
            await _customersService.UpdateAsset(cmpId, company);
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

        public async Task<IActionResult> CreateGroup(string id, IndexViewModel vm)
        {
            var cmp = _customersService.GetCompany(id);
            if (cmp.Groups.Contains(vm.NewGroup) == false)
            {
                cmp.Groups.Add(vm.NewGroup);
                await _customersService.UpdateAsset(id, cmp);
            }

            return RedirectToAction("CompanyProfile", "AdminPanel", new { id = id });
        }

        [HttpPost]
        public IActionResult CreateUser(string compyid, IndexViewModel vm)
        {
            CreateNewUser(compyid, vm.NewUser.UserName, vm.NewUser.FirstName, vm.NewUser.LastName, vm.NewUser.Email, vm.NewUsrPass, "", false);

            return RedirectToAction("CompanyProfile", "AdminPanel", new { id = compyid });
        }

        [HttpPost]
        public IActionResult CreateUsersFromCSV(string compyid, IndexViewModel vm)
        {
            // ToDo : Add controls to ensure that bad file or format are managed
            try
            {
                if (vm.NewUsrCSV == null)
                {
                    throw new FileNotFoundException("CSV file not found for users import");
                }

                using (var reader = new StreamReader(vm.NewUsrCSV.OpenReadStream()))
                {
                    string[] splitted;
                    // Skip header line
                    string? l = reader.ReadLine();
                    l = reader.ReadLine();
                    while (!string.IsNullOrEmpty(l))
                    {
                        splitted = l.Split(",");
                        if (splitted.Count() != 7)
                        {
                            throw new FormatException("CSV file format invalid for users import : Incorrect number of elements");
                        }

                        bool activateUsr = splitted[6].ToLower() == "yes";
                        CreateNewUser(compyid, splitted[0], splitted[1], splitted[2], splitted[3], splitted[4], splitted[5], activateUsr);

                        _logger.LogInformation("ParseCSV : Username={0} :: Email={1} :: Password={2} :: Group={3} :: Active={4}",
                                                splitted[0], splitted[3], splitted[4], splitted[5], splitted[6]);
                        l = reader.ReadLine();
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("CompanyProfile", "AdminPanel", new { id = compyid });
            }
            
            return RedirectToAction("CompanyProfile", "AdminPanel", new { id = compyid });
        }

        [HttpPost]
        public IActionResult ActivateUser(string compyid, string usrid, IndexViewModel vm)
        {
            var usrInfo = _userManager.FindByIdAsync(usrid).Result;
            var cmpny = _customersService.GetCompany(compyid);
            if (cmpny == null)
            {
                _logger.LogWarning("Failed to update user, company not found :: CompanyId={0} :: UserId={1}", compyid, usrid);
                return RedirectToAction("CompanyProfile", "AdminPanel", new { id = compyid });
            }

            var nbUsrs = _userManager.Users.ToList().FindAll(x => x.IsActive == true).Count();
            if (vm.NewUser.IsActive && nbUsrs >= cmpny.MaxUsers)
            {
                _logger.LogWarning("Failed to update user, too many active users :: CompanyId={0} :: UserId={1}", compyid, usrid);
                return RedirectToAction("CompanyProfile", "AdminPanel", new { id = compyid });
            }

            usrInfo.IsActive = vm.NewUser.IsActive;

            var resUpdate = _userManager.UpdateAsync(usrInfo).Result;
            if (resUpdate.Succeeded)
            {
                _logger.LogInformation("User activity updated :: CompanyId={0} :: UserId={1} :: Activity={2}",
                                        compyid, usrid, usrInfo.IsActive.ToString());
            }
            else
            {
                _logger.LogWarning("User activity update failed :: CompanyId={0} :: UserId={1} :: Activity={2}",
                                    compyid, usrid, usrInfo.IsActive.ToString());
            }

            return RedirectToAction("CompanyProfile", "AdminPanel", new { id = compyid });
        }

        [HttpPost]
        public IActionResult ChangeUserGroup(string compyid, string usrid, IndexViewModel vm)
        {
            var usrInfo = _userManager.FindByIdAsync(usrid).Result;
            var cmpny = _customersService.GetCompany(compyid);
            if (cmpny == null)
            {
                _logger.LogWarning("Failed to update user, company not found :: CompanyId={0} :: UserId={1}", compyid, usrid);
                return RedirectToAction("CompanyProfile", "AdminPanel", new { id = compyid });
            }

            usrInfo.GroupId = vm.NewUser.GroupId;

            var resUpdate = _userManager.UpdateAsync(usrInfo).Result;
            if (resUpdate.Succeeded)
            {
                _logger.LogInformation("User group updated :: CompanyId={0} :: UserId={1} :: Group={2}",
                                        compyid, usrid, usrInfo.GroupId);
            }
            else
            {
                _logger.LogWarning("User group update failed :: CompanyId={0} :: UserId={1} :: Group={2}",
                                    compyid, usrid, usrInfo.GroupId);
            }

            return RedirectToAction("CompanyProfile", "AdminPanel", new { id = compyid });
        }

        /// <summary>
        /// Creates a new user in the database. User will have a "USER" role
        /// </summary>
        /// <param name="cmpId">ID of the company the user it employed by</param>
        /// <param name="usrName">Full name of the user</param>
        /// <param name="usrMail">Email address of the user</param>
        /// <param name="usrPass">Password of the account to create. If void, the default password will be assigned</param>
        /// <param name="usrGroup">The company group to assign the user to</param>
        /// <param name="active">Wether the user should be activated</param>
        /// <returns></returns>
        private bool CreateNewUser(string cmpId, string usrName, string firstName, string lastName, string usrMail, string usrPass,
                                   string usrGroup, bool active)
        {
            if (string.IsNullOrEmpty(cmpId) || string.IsNullOrEmpty(usrName) || string.IsNullOrEmpty(usrMail))
            {
                // ToDo : Make validation of elements
                return false;
            }

            // ToDo : Test Metamask Addr as well
            if (_userManager.Users.Any(u => u.Email == usrMail))
            {
                _logger.LogWarning("Cannot create user, already exists :: CompanyId={0} :: Name={1} :: Email={2}", cmpId, usrName, usrMail);
                return false;
            }

            if (string.IsNullOrEmpty(usrPass))
            {
                usrPass = "NancyFlocon&2021";
            }

            var cmpny = _customersService.GetCompany(cmpId);
            if (cmpny == null)
            {
                _logger.LogWarning("Failed to create user, company not found :: CompanyId={0} :: Name={1} :: Email={2}", cmpId, usrName, usrMail);
                return false;
            }

            var myUser = new UserFlocon { UserName = usrName, FirstName=firstName, LastName=lastName, Email = usrMail, CompanyId = cmpId};
            if (!string.IsNullOrEmpty(usrGroup))
            {
                // Assign group
                if (cmpny.Groups.Contains(usrGroup))
                {
                    myUser.GroupId = usrGroup;
                }
                else
                {
                    _logger.LogWarning("Failed to assign group to user, group not found :: CompanyId={0} :: Name={1} :: Email={2} :: Group={3}", 
                                        cmpId, usrName, usrMail, usrGroup);
                    myUser.GroupId = "";
                }
            }

            var nbUsrs = _userManager.Users.ToList().FindAll(x => x.IsActive == true).Count();
            if (active && nbUsrs >= cmpny.MaxUsers)
            {
                _logger.LogWarning("Too many active users, set to inactive :: CompanyId={0} :: Name={1} :: Email={2}", cmpId, usrName, usrMail);
                myUser.IsActive = false;
            }
            else
            {
                myUser.IsActive = active;
            }

            var resCreate = _userManager.CreateAsync(myUser, usrPass).Result;
            if (resCreate.Succeeded)
            {
                var resRole = _userManager.AddToRoleAsync(myUser, "User").Result;
                if (resRole.Succeeded == false)
                {
                    _logger.LogWarning("Failed to attribute User role at creation :: CompanyId={0} :: Name={1} :: Email={2} :: Pass={3} :: Group={4}",
                                        cmpId, usrName, usrMail, usrPass, usrGroup);
                    return false;
                }
            }
            else
            {
                _logger.LogWarning("Failed to create user :: CompanyId={0} :: Name={1} :: Email={2} :: Pass={3} :: Group={4}",
                                   cmpId, usrName, usrMail, usrPass, usrGroup);
                return false;
            }

            _logger.LogInformation("User created :: CompanyId={0} :: Name={1} :: Email={2} :: Pass={3} :: Group={4} :: Active={5}",
                                   myUser.CompanyId, myUser.UserName, myUser.Email, usrPass, myUser.GroupId, myUser.IsActive.ToString());

            return true;
        }

        // ToDo : Move to a "common" library
        /// <summary>
        /// Builds a random password for new users. Has 12 characters (3 upper case letters, 3 lower case letters, 3 digits, and 3 special characters)
        /// </summary>
        /// <returns>The randomly generated password</returns>
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

        /// <summary>
        /// Gets the user currently connected to display user infos
        /// </summary>
        /// <returns>Currently connected user or null if none connected</returns>
        public async Task<UserFlocon> GetConnectedUser()
        {
            ClaimsPrincipal currentUser = this.User;
            var usr = await _userManager.GetUserAsync(User);

            return usr;
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