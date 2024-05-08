using System.Xml.Serialization;

#nullable disable
namespace EasyKeys.Shipping.Usps.Tracking.Models
{
    [XmlRoot(ElementName = "TrackRequest")]
    public class TrackRequest
    {
        [XmlElement(ElementName = "TrackID")]
        public List<TrackID> TrackID { get; set; }

        [XmlAttribute(AttributeName = "USERID")]
        public string USERID { get; set; }
    }
}
