using System.Xml.Serialization;

#nullable disable
namespace EasyKeys.Shipping.Usps.Tracking.Models
{
    [XmlRoot(ElementName = "TrackID")]
    public class TrackID
    {
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }
    }
}
