using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Flocon.Services.Mailing;
using System.IO;
using System.Threading.Tasks;

using RazorEngine;
using RazorEngine.Templating;
using RazorEngine.Configuration;

using Flocon.Models;
using RazorEngine.Text;

namespace Flocon.Services.Mailing
{
    public class EmailSender : IEmailSender
    {
        public string GenerateEmail(MailFields mf)
        {
            string templatePath = @"wwwroot/content/EmailLayout.txt";
            var reader = new StreamReader(templatePath);

            var _html = reader.ReadToEnd();

            if (string.IsNullOrEmpty(_html))
            {
                return "";
            }

            var config = new TemplateServiceConfiguration();
            config.Debug = true;
            config.Language = Language.CSharp;
            config.EncodedStringFactory = new HtmlEncodedStringFactory(); // Html encoding.
            //config.DisableTempFileLocking = true;
            //config.TemplateManager = new ResolvePathTemplateManager();
            Engine.Razor = RazorEngineService.Create(config);

            var result = Engine.Razor.RunCompile(_html, "FloconMail", null, mf);

            return result;
        }


        public Task SendResetPwdEmailAsync(string email, string usrname, string resetUrl)
        {
            //await _emailSender.SendEmailAsync(model.Email, user.UserName, "Reset Password",
            //       $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
            var sendGridKey = @"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";
            return ExecuteResetPwd(sendGridKey, email, usrname, resetUrl);
        }

        public async Task ExecuteResetPwd(string apiKey, string email, string usrname, string resetUrl)
        {
            // ToDo : Make proper HTML email
            // ToDo : Remove my personal email

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("neant.total@gmail.com", "Jul");
            var to = new EmailAddress(email, usrname);
            string subject = "Account Recovery - SilverWallet";
            string txtMsg = string.Format("Hello {0}! We received an account recovery request on Stack Overflow for {1}. If you initiated this request, reset your password here : {2}", usrname, email, resetUrl);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, txtMsg, txtMsg);

            // Disable click tracking.
            msg.SetClickTracking(false, false);

            // ToDo : Put back email management
            var response = await client.SendEmailAsync(msg);

            // return response;
        }

        public Task SendContactUsEmailAsync(string email, string nickname, string subject, string msg)
        {
            var sendGridKey = @"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";

            var mf = new MailFields
            {
                Title = "Contact message to Flocon.io",
                AddImage = false,
                SrcImage = "",
                SupHeader = $"{nickname}",
                Header = $"{subject}",
                Paragraph = $"{msg}",
                ActionButton = "Reply",
                ActionTxt = "Reply",
                ActionLink = $"mailto:{email}"
            };

            // ToDo : Replace mail with Flocon contact mail
            return ExecuteMail(sendGridKey, "neant.total@gmail.com", "Flocon Admin", mf);
        }

        public Task SendRegisterEmailAsync(string email, string nickname, string company, string password, string validUrl)
        {
            var sendGridKey = @"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";

            var mf = new MailFields
            {
                Title = "Welcome to Flocon.io",
                AddImage = false,
                SrcImage = "",
                SupHeader = $"{nickname} - {company}",
                Header = "Welcome to Flocon!",
                Paragraph = $"You have been subscribed to Flocon under the company {company}. Your temporary password is: {password} Please change it once signed in, and verify your email address by clicking on the link below.",
                ActionButton = "Verify your Email",
                ActionTxt = "Email verification link: ",
                ActionLink = "http://flocon.io"
            };

            return ExecuteMail(sendGridKey, email, nickname, mf);
        }

        public async Task ExecuteMail(string apiKey, string toMail, string toName, MailFields mf)
        {
            // ToDo : Test and adjust
            // ToDo : Make proper HTML email
            // ToDo : Remove my personal email

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("neant.total@gmail.com", "Jul");
            var to = new EmailAddress(toMail, toName);
            
            string subject = mf.Title;
            string htmlMsg = GenerateEmail(mf);
            string txtMsg = $"{mf.Header} {mf.Paragraph} {mf.ActionTxt} {mf.ActionLink}";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, txtMsg, htmlMsg);

            // Disable click tracking.
            msg.SetClickTracking(false, false);

            // ToDo : Put back email management
            var response = await client.SendEmailAsync(msg);
            var test = 0;
        }
    }
}