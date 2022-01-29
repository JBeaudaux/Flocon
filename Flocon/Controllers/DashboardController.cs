using AspNetCore.Identity.Mongo.Model;
using Flocon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Flocon.Controllers
{
    [Authorize(Roles = "User")]
    public class DashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<UserFlocon> _userManager;
        private readonly RoleManager<MongoRole> _roleManager;
        private readonly SignInManager<UserFlocon> _signInManager;

        private UserFlocon _userConnected;

        public DashboardController(UserManager<UserFlocon> userManager, RoleManager<MongoRole> roleManager, SignInManager<UserFlocon> signInManager, ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;

            if (User != null && User.Identity != null)
            {
                _userConnected = userManager.FindByNameAsync(User.Identity.Name).Result;
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}