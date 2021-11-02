using System.Xml.Serialization;

namespace EasyKeys.Shipping.Usps.Tracking.Models
{
    [XmlRoot(ElementName = "TrackID")]
    public class TrackID
    {
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }
    }
}
