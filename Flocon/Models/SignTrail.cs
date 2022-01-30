using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Flocon.Models
{
    public class SignTrail
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string CreatorId { get; set; } = "";
        public string FileName { get; set; } = "";
        public string LocalStorageName { get; set; } = "";
        public string FileHash { get; set; } = "";

        public string DocName { get; set; } = "";
        public uint DocVersion { get; set; }

        public string SignWriterId { get; set; } = "";
        public string SignWriterTx { get; set; } = "";
        public string SignWriterUrl { get; set; } = "";
        
        public string SignApproverId { get; set; } = "";
        public string SignApproverTx { get; set; } = "";
        public string SignApproverUrl { get; set; } = "";

        /*------------ Parameters ------------*/

        public bool UploadOriginalIPFS { get; set; } = false;
        public string OriginalIPFS { get; set; } = "";
        public bool UploadSignedIPFS { get; set; } = true;
        public string SignedIPFS { get; set; } = "";

        /*------------ STATISTICS ------------*/
        public DateTime CreatedOn { get; set; }
    }
}
