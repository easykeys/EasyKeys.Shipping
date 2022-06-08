namespace EasyKeys.Shipping.Stamps.Shipment.Models;

public class CustomsInformation
{
    public string CertificateNumber { get; set; } = string.Empty;

    public string InvoiceNumber { get; set; } = string.Empty;

    public string OtherDescribe { get; set; } = string.Empty;

    public string CustomsSigner { get; set; } = string.Empty;

    public string PassportNumber { get; set; } = string.Empty;

    public DateTime PassportIsseDate { get; set; }

    public DateTime PassportExpiryDate { get; set; }

    public string ImportLicenseNumber { get; set; } = string.Empty;

    public string SendersCustomsReference { get; set; } = string.Empty;
}
