﻿using AspNetCore.Identity.Mongo.Model;
using Flocon.Models;
using Flocon.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Flocon.Controllers
{
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

        // Role creation
        /*
        MongoRole role = new MongoRole();
        role.Name = "User";
        IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
        */

        // User creation
        /*
        var myUser = new UserFlocon { UserName="Yojik", Email="julienbeaudaux@gmail.com", EmailConfirmed=true };
        var resCreate = _userManager.CreateAsync(myUser, "PassWrd@863").Result;
        var resRole = _userManager.AddToRoleAsync(myUser, "DemoUser").Result;
        */

        [HttpGet]
        public IActionResult Login()
        {
            // If a user is already authenticated, go straight to Dashboard
            /*if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }*/

            var vm = new LoginViewModel();
            vm.MetamaskAddr = "";

            return View(vm);
        }

        [HttpPost]
        public IActionResult MetamaskLogin(LoginViewModel vm)
        {
            return View("login");
        }

        // Used to capture the login
        /*[HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {

            return RedirectToAction("Login", "Auth");
        }*/


        /*
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        */
    }
}