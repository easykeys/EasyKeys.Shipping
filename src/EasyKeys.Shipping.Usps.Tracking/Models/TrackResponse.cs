using System.Xml.Serialization;

#nullable disable
namespace EasyKeys.Shipping.Usps.Tracking.Models
{
    [XmlRoot(ElementName = "TrackResponse")]
    public class TrackResponse
    {
        [XmlElement(ElementName = "TrackInfo")]
        public List<TrackInfo> TrackInfo { get; set; }
    }
}
