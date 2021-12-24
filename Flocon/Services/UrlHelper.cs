using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Flocon.Controllers;

namespace Flocon.Services
{
    public static class UrlHelper
    {
        // ToDo : Make a rea verification code
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            /*return urlHelper.Action(
                action: nameof(AuthController.ConfirmEmail),
                controller: "Auth",
                values: new { userId, code },
                protocol: scheme);*/
            return "http://flocon.io";
        }

        
        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            /*return urlHelper.Action(
                action: nameof(AuthController.ResetPassword),
                controller: "Auth",
                values: new { userId, code },
                protocol: scheme);*/
            return "http://flocon.io";
        }
        
    }
}
