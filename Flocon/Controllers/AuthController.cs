using AspNetCore.Identity.Mongo.Model;
using Flocon.Models;
using Flocon.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Flocon.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private readonly UserManager<UserFlocon> _userManager;
        private readonly RoleManager<MongoRole> _roleManager;
        private readonly SignInManager<UserFlocon> _signInManager;
        private readonly ILogger<HomeController> _logger;

        public AuthController(UserManager<UserFlocon> userManager,
                              RoleManager<MongoRole> roleManager,
                              SignInManager<UserFlocon> signInManager,
                              ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        #region "Web method"

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // If a user is already authenticated, go straight to Dashboard
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> MetamaskLogin(LoginViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.MetamaskAddr))
            {
                return LoginPasswordIncorrect();
            }
            var usr = _userManager.Users.FirstOrDefault<UserFlocon>(x => x.MetamaskAddr.ToLower() == vm.MetamaskAddr);

            if (usr == null)
            {
                return LoginPasswordIncorrect();
            }
            else
            {
                await _signInManager.SignInAsync(usr, false, null);

                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UserPasswordLogin(LoginViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.LoginEmail) || string.IsNullOrEmpty(vm.LoginPwd))
            {
                return LoginPasswordIncorrect();
            }

            var usr = await _userManager.FindByEmailAsync(vm.LoginEmail);

            if (usr == null)
            {
                return LoginPasswordIncorrect();
            }
            else
            {
                var result = await _signInManager.PasswordSignInAsync(usr, vm.LoginPwd, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    return LoginPasswordIncorrect();
                }
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Auth", "Login"); // After logout, goes to homepage
        }

        #endregion "Web method"

        #region "Methods"

        private Boolean CheckPasswordAgainstPolicy()
        {
            return false;
        }

        private ViewResult LoginPasswordIncorrect()
        {
            _logger.LogInformation("Login/Password is incorrect.");
            return View("Login");
        }

        #endregion "Methods"
    }
}