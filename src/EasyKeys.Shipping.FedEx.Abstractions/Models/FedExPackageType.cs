using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.FedEx.Abstractions.Models;

public class FedExPackageType : SmartEnum<FedExPackageType>
{
    public static readonly FedExPackageType FedExEnvelope = new("FEDEX_ENVELOPE", 0);
    public static readonly FedExPackageType YourPackaging = new("YOUR_PACKAGING", 1);
    public static readonly FedExPackageType FedExPak = new("FEDEX_PAK", 2);
    public static readonly FedExPackageType FedEx10kgBox = new("FEDEX_10KG_BOX", 3);
    public static readonly FedExPackageType FedEx25kgBox = new("FEDEX_25KG_BOX", 4);
    public static readonly FedExPackageType FedExBox = new("FEDEX_BOX", 5);
    public static readonly FedExPackageType FedExExtraLargeBox = new("FEDEX_EXTRA_LARGE_BOX", 6);
    public static readonly FedExPackageType FedExLargeBox = new("FEDEX_LARGE_BOX", 7);
    public static readonly FedExPackageType FedExMediumBox = new("FEDEX_MEDIUM_BOX", 8);
    public static readonly FedExPackageType FedExSmallBox = new("FEDEX_SMALL_BOX", 9);
    public static readonly FedExPackageType FedExTube = new("FEDEX_TUBE", 10);

    protected FedExPackageType(string name, int value) : base(name, value)
    {
    }
}
