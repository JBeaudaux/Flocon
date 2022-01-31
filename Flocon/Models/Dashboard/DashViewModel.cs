using AspNetCore.Identity.Mongo.Model;
using Flocon.Models;

namespace Flocon.Models.Dashboard
{
    public class DashViewModel
    {
        public IFormFile DocToSign { get; set; }

        public SignTrail SignDoc { get; set; }

        public string PdfPath { get; set; } = string.Empty;

        public UserFlocon? UserFlocon { get; set; }

        public List<SignTrail> UserDocuments { get; set; } = new List<SignTrail>();
    }
}