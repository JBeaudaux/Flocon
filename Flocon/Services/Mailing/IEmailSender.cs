using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flocon.Services.Mailing
{
    public interface IEmailSender
    {
        Task SendRegisterEmailAsync(string email, string nickname, string company, string password, string validUrl);
        Task SendResetPwdEmailAsync(string email, string nickname, string validUrl);
        Task SendContactUsEmailAsync(string email, string nickname, string subject, string msg);
    }
}
