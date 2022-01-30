using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IFlocon.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class SecuredController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}