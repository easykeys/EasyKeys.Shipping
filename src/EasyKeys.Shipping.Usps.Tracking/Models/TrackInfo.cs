using System.Xml.Serialization;

#nullable disable

namespace EasyKeys.Shipping.Usps.Tracking.Models
{
    [XmlRoot(ElementName = "TrackInfo")]
    public class TrackInfo
    {
        [XmlElement(ElementName = "CarrierRelease")]
        public string CarrierRelease { get; set; }

        [XmlElement(ElementName = "Class")]
        public string Class { get; set; }

        [XmlElement(ElementName = "ClassOfMailCode")]
        public string ClassOfMailCode { get; set; }

        [XmlElement(ElementName = "DestinationCity")]
        public string DestinationCity { get; set; }

        [XmlElement(ElementName = "DestinationState")]
        public string DestinationState { get; set; }

        [XmlElement(ElementName = "DestinationZip")]
        public string DestinationZip { get; set; }

        [XmlElement(ElementName = "EmailEnabled")]
        public string EmailEnabled { get; set; }

        [XmlElement(ElementName = "EnabledNotificationRequests")]
        public EnabledNotificationRequests EnabledNotificationRequests { get; set; }

        [XmlElement(ElementName = "Error")]
        public Error Error { get; set; }

        [XmlElement(ElementName = "eSOFEligible")]
        public string ESOFEligible { get; set; }

        [XmlElement(ElementName = "ItemShape")]
        public string ItemShape { get; set; }

        [XmlElement(ElementName = "KahalaIndicator")]
        public string KahalaIndicator { get; set; }

        [XmlElement(ElementName = "MailTypeCode")]
        public string MailTypeCode { get; set; }

        [XmlElement(ElementName = "MPDATE")]
        public string MPDATE { get; set; }

        [XmlElement(ElementName = "MPSUFFIX")]
        public string MPSUFFIX { get; set; }

        [XmlElement(ElementName = "OnTime")]
        public string OnTime { get; set; }

        [XmlElement(ElementName = "OriginCity")]
        public string OriginCity { get; set; }

        [XmlElement(ElementName = "OriginState")]
        public string OriginState { get; set; }

        [XmlElement(ElementName = "OriginZip")]
        public string OriginZip { get; set; }

        [XmlElement(ElementName = "PodEnabled")]
        public string PodEnabled { get; set; }

        [XmlElement(ElementName = "TPodEnabled")]
        public string TPodEnabled { get; set; }

        [XmlElement(ElementName = "RedeliveryEnabled")]
        public string RedeliveryEnabled { get; set; }

        [XmlElement(ElementName = "RestoreEnabled")]
        public string RestoreEnabled { get; set; }

        [XmlElement(ElementName = "RramEnabled")]
        public string RramEnabled { get; set; }

        [XmlElement(ElementName = "RreEnabled")]
        public string RreEnabled { get; set; }

        [XmlElement(ElementName = "Service")]
        public string Service { get; set; }

        [XmlElement(ElementName = "ServiceTypeCode")]
        public string ServiceTypeCode { get; set; }

        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "StatusCategory")]
        public string StatusCategory { get; set; }

        [XmlElement(ElementName = "StatusSummary")]
        public string StatusSummary { get; set; }

        [XmlElement(ElementName = "TABLECODE")]
        public string TABLECODE { get; set; }

        [XmlElement(ElementName = "TrackSummary")]
        public TrackSummary TrackSummary { get; set; }

        [XmlElement(ElementName = "TrackDetail")]
        public List<TrackDetail> TrackDetail { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }
    }

    [XmlRoot(ElementName = "EnabledNotificationRequests")]
    public class EnabledNotificationRequests
    {
        [XmlElement(ElementName = "SMS")]
        public SMS SMS { get; set; }

        [XmlElement(ElementName = "Email")]
        public Email Email { get; set; }
    }

    [XmlRoot(ElementName = "SMS")]
    public class SMS
    {
        [XmlElement(ElementName = "FD")]
        public string FD { get; set; }

        [XmlElement(ElementName = "AL")]
        public string AL { get; set; }

        [XmlElement(ElementName = "TD")]
        public string TD { get; set; }

        [XmlElement(ElementName = "UP")]
        public string UP { get; set; }

        [XmlElement(ElementName = "DND")]
        public string DND { get; set; }

        [XmlElement(ElementName = "OA")]
        public string OA { get; set; }

        [XmlElement(ElementName = "FS")]
        public string FS { get; set; }
    }

    [XmlRoot(ElementName = "Email")]
    public class Email
    {
        [XmlElement(ElementName = "FD")]
        public string FD { get; set; }

        [XmlElement(ElementName = "AL")]
        public string AL { get; set; }

        [XmlElement(ElementName = "TD")]
        public string TD { get; set; }

        [XmlElement(ElementName = "UP")]
        public string UP { get; set; }

        [XmlElement(ElementName = "DND")]
        public string DND { get; set; }

        [XmlElement(ElementName = "OA")]
        public string OA { get; set; }

        [XmlElement(ElementName = "FS")]
        public string FS { get; set; }
    }
}
