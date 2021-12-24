using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Flocon.Models
{
    public class MailFields
    {
        public string Title { get; set; }
        public bool AddImage { get; set; }
        public string SrcImage { get; set; }
        public string SupHeader { get; set; }
        public string Header { get; set; }
        public string Paragraph { get; set; }
        public string ActionLink { get; set; }
        public string ActionButton { get; set; }
        public string ActionTxt { get; set; }
    }
}
