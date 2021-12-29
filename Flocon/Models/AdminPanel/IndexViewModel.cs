using AspNetCore.Identity.Mongo.Model;

namespace Flocon.Models.AdminPanel
{
    public class IndexViewModel
    {
        /* -------------- DATA MANAGEMENT -------------- */
        public List<UserFlocon> Users { get; set; }
        public List<MongoRole> Roles { get; set; }
        public List<Company> Companies { get; set; }

        /* -------------- FORMS MODELS -------------- */
        public Company NewCompany { get; set; }
        public string NewGroup { get; set; }
        public IFormFile NewAvatar { get; set; }

        public UserFlocon NewUser { get; set; }
        // ToDo : replace name, fist name, last name, mail with NewUser
        public string NewUsrPass { get; set; }
        public IFormFile NewUsrCSV { get; set; }
    }
}