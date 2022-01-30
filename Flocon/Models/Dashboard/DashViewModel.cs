using AspNetCore.Identity.Mongo.Model;

namespace Flocon.Models.Dashboard
{
    public class DashViewModel
    {
        public IFormFile DocToSign { get; set; }

        public SignTrail SignDoc { get; set; }

        public string PdfPath { get; set; } = string.Empty;
    }
}