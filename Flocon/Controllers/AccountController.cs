using AspNetCore.Identity.Mongo.Model;
using Flocon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Flocon.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<UserFlocon> _userManager;
        private SignInManager<UserFlocon> _signInManager;

        public AccountController(UserManager<UserFlocon> userManager, SignInManager<UserFlocon> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}