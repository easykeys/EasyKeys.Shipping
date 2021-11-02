using System.Collections.Generic;
using System.Xml.Serialization;

namespace EasyKeys.Shipping.Usps.Tracking.Models
{
    [XmlRoot(ElementName = "TrackResponse")]
    public class TrackResponse
    {
        [XmlElement(ElementName = "TrackInfo")]
        public List<TrackInfo> TrackInfo { get; set; }
    }
}
