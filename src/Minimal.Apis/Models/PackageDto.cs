namespace Minimal.Apis.Models;

public class PackageDto
{
    public DateTime? ShipDate { get; set; }

    public bool SignatureRequiredOnDelivery { get; set; }

    public decimal InsuredValue { get; set; }

    public decimal Length { get; set; }

    public decimal Width { get; set; }

    public decimal Height { get; set; }

    public decimal Weight { get; set; }
}
