using System.Xml.Serialization;

#nullable disable
namespace EasyKeys.Shipping.Usps.Tracking.Models
{
    [XmlRoot(ElementName = "TrackSummary")]
    public class TrackSummary
    {
        [XmlElement(ElementName = "EventTime")]
        public string EventTime { get; set; }

        [XmlElement(ElementName = "EventDate")]
        public string EventDate { get; set; }

        [XmlElement(ElementName = "Event")]
        public string Event { get; set; }

        [XmlElement(ElementName = "EventCity")]
        public string EventCity { get; set; }

        [XmlElement(ElementName = "EventState")]
        public string EventState { get; set; }

        [XmlElement(ElementName = "EventZIPCode")]
        public string EventZIPCode { get; set; }

        [XmlElement(ElementName = "EventCountry")]
        public string EventCountry { get; set; }

        [XmlElement(ElementName = "FirmName")]
        public string FirmName { get; set; }

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "AuthorizedAgent")]
        public string AuthorizedAgent { get; set; }

        [XmlElement(ElementName = "EventCode")]
        public string EventCode { get; set; }

        [XmlElement(ElementName = "EventStatusCategory")]
        public string EventStatusCategory { get; set; }
    }
}
