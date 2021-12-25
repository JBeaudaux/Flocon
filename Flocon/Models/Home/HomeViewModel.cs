using AspNetCore.Identity.Mongo.Model;

namespace Flocon.Models.Home
{
    public class HomeViewModel
    {
        /* -------------- Contact Us -------------- */
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}