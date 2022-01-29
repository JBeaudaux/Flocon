using AspNetCore.Identity.Mongo.Model;

namespace Flocon.Models.Auth
{
    public class LoginViewModel
    {
        /* -------------- DATA MANAGEMENT -------------- */
        public string MetamaskAddr { get; set; } = string.Empty;

        /* -------------- FORMS MODELS -------------- */
        public string LoginEmail { get; set; } = string.Empty;
        public string LoginPwd { get; set; } = string.Empty;
    }
}