using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson.Serialization.Attributes;

namespace Flocon.Models
{
    [BsonIgnoreExtraElements]
    public class UserFlocon : MongoUser
    {
        public string FistName { get; set; }
        public string LastName { get; set; }
        public string MetamaskAddr { get; set; }
        public string ProfilePicPath { get; set; }

        // ToDo : replace GroupId, also make it so we can be in several groups
        public string GroupId { get; set; }
        public string CompanyId { get; set; }
        public bool IsActive { get; set; }
    }
}
