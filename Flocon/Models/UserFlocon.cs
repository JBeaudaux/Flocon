using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Flocon.Models
{
    [BsonIgnoreExtraElements]
    public class UserFlocon : MongoUser
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public override string? Email { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MetamaskAddr { get; set; }
        public string? ProfilePicPath { get; set; }

        public List<string>? RoleID { get; set; }

        // ToDo : replace GroupId, also make it so we can be in several groups
        public string? GroupId { get; set; }

        public string? CompanyId { get; set; }
        public bool IsActive { get; set; }
    }
}