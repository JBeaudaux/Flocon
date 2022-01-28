using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Flocon.Models
{
    public class Company
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string CompanyName { get; set; }
        public string BusinessField { get; set; }
        public string Description { get; set; }
        public string LogoPath { get; set; }

        /*------------ CONTACT INFO ------------*/
        public string ContactName { get; set; }
        public string ContactMail { get; set; }
        public string ContactPhone { get; set; }
        public string ContactAddress { get; set; }
        public string ContactWebpage { get; set; }
        public string ContactLinkedin { get; set; }

        /*------------ LICENCE ------------*/
        public string LicenceType { get; set; }

        public int MaxUsers { get; set; }
        public DateTime LicenceExpiry { get; set; }

        /*------------ GROUPS ------------*/
        public List<string> Groups { get; set; }

        /*------------ STATISTICS ------------*/
        public DateTime CreatedOn { get; set; }
    }
}
