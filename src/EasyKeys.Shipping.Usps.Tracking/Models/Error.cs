using System.Xml.Serialization;

namespace EasyKeys.Shipping.Usps.Tracking.Models
{
    [XmlRoot(ElementName = "Error")]
    public class Error
    {
        [XmlElement(ElementName = "Number")]
        public string? Number { get; set; }

        [XmlElement(ElementName = "Source")]
        public string? Source { get; set; }

        [XmlElement(ElementName = "Description")]
        public string? Description { get; set; }

        [XmlElement(ElementName = "HelpFile")]
        public string? HelpFile { get; set; }

        [XmlElement(ElementName = "HelpContext")]
        public string? HelpContext { get; set; }
    }
}
