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
    }
}