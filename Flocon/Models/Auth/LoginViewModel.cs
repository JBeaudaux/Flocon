using AspNetCore.Identity.Mongo.Model;

namespace Flocon.Models.Auth
{
    public class LoginViewModel
    {
        /* -------------- DATA MANAGEMENT -------------- */
        public string MetamaskAddr { get; set; }

        /* -------------- FORMS MODELS -------------- */
        public string LoginEmail { get; set; }
        public string LoginPwd { get; set; }

    }
}