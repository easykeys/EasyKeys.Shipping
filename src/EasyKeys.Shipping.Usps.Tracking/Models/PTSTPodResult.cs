using System.Xml.Serialization;

namespace EasyKeys.Shipping.Usps.Tracking.Models
{
    [XmlRoot(ElementName = "PTSTPODRESULT")]
    public class PTSTPODRESULT
    {
        [XmlElement(ElementName = "ResultText")]
        public string ResultText { get; set; }

        [XmlElement(ElementName = "ReturnCode")]
        public string ReturnCode { get; set; }
    }
}
