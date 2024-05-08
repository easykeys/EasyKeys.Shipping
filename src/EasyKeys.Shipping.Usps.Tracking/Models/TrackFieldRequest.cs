using System.Xml.Serialization;

#nullable disable
namespace EasyKeys.Shipping.Usps.Tracking.Models
{
    [XmlRoot(ElementName = "TrackFieldRequest")]
    public class TrackFieldRequest
    {
        [XmlAttribute(AttributeName = "USERID")]
        public string USERID { get; set; }

        [XmlElement(ElementName = "Revision")]
        public string Revision { get; set; }

        [XmlElement(ElementName = "ClientIp")]
        public string ClientIp { get; set; }

        [XmlElement(ElementName = "SourceId")]
        public string SourceId { get; set; }

        [XmlElement(ElementName = "TrackID")]
        public List<TrackID> TrackID { get; set; } // todo: unsure if this can take a list
    }
}
