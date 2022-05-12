namespace EasyKeys.Shipping.Stamps.API.Models;

public class PackageDto
{
    public bool SignatureRequiredOnDelivery { get; set; }

    public decimal InsuredValue { get; set; }

    public decimal Length { get; set; }

    public decimal Width { get; set; }

    public decimal Height { get; set; }

    public decimal Weight { get; set; }
}
