using System.Xml.Serialization;

#nullable disable
namespace EasyKeys.Shipping.Usps.Tracking.Models
{
    [XmlRoot(ElementName = "PTSPodRequest")]
    public class PTSPodRequest
    {
        [XmlElement(ElementName = "TrackId")]
        public string TrackId { get; set; }

        [XmlElement(ElementName = "ClientIp")]
        public string ClientIp { get; set; }

        [XmlElement(ElementName = "SourceId")]
        public string SourceId { get; set; }

        [XmlElement(ElementName = "MpSuffix")]
        public string MpSuffix { get; set; }

        [XmlElement(ElementName = "MpDate")]
        public string MpDate { get; set; }

        [XmlElement(ElementName = "RequestType")]
        public string RequestType { get; set; }

        [XmlElement(ElementName = "FirstName")]
        public string FirstName { get; set; }

        [XmlElement(ElementName = "LastName")]
        public string LastName { get; set; }

        [XmlElement(ElementName = "Email1")]
        public string Email1 { get; set; }

        [XmlElement(ElementName = "Email2")]
        public string Email2 { get; set; }

        [XmlElement(ElementName = "Email3")]
        public string Email3 { get; set; }

        [XmlElement(ElementName = "TableCode")]
        public string TableCode { get; set; }

        [XmlElement(ElementName = "CustRegID")]
        public string CustRegID { get; set; }

        [XmlAttribute(AttributeName = "USERID")]
        public string USERID { get; set; }
    }
}
