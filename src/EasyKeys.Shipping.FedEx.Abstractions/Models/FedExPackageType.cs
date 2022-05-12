using Ardalis.SmartEnum;

using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Abstractions.Models;

/// <summary>
/// <see href="https://www.fedex.com/en-us/shipping/one-rate.html#packing-supplies"/>.
/// </summary>
public abstract class FedExPackageType : SmartEnum<FedExPackageType>
{
    public static readonly FedExPackageType FedExEnvelope = new FedExEnvelopeType();
    public static readonly FedExPackageType YourPackaging = new YourPackagingType();
    public static readonly FedExPackageType FedExPak = new FedExPakType();
    public static readonly FedExPackageType FedEx10kgBox = new FedEx10kgBoxType();
    public static readonly FedExPackageType FedEx25kgBox = new FedEx25kgBoxType();
    public static readonly FedExPackageType FedExBox = new FedExBoxType();
    public static readonly FedExPackageType FedExExtraLargeBox = new FedExExtraLargeBoxType();
    public static readonly FedExPackageType FedExLargeBox = new FedExLargeBoxType();
    public static readonly FedExPackageType FedExMediumBox = new FedExMediumBoxType();
    public static readonly FedExPackageType FedExSmallBox = new FedExSmallBoxType();
    public static readonly FedExPackageType FedExTube = new FedExTubeType();

    protected FedExPackageType(string name, int value) : base(name, value)
    {
    }

    public abstract Dimensions Dimensions { get; }

    public abstract decimal MaxWeight { get; }

    public abstract decimal MinWeight { get; }

    /// <summary>
    /// FEDEX_ENVELOPE 10 lbs l: 9 1/2 (9.50) x w:12 1/2 (12.50).
    /// </summary>
    private class FedExEnvelopeType : FedExPackageType
    {
        public FedExEnvelopeType() : base("FEDEX_ENVELOPE", 0)
        {
        }

        public override Dimensions Dimensions => new Dimensions(9m, 12m, 0.25m);

        public override decimal MaxWeight => 1m;

        public override decimal MinWeight => 0.1m;
    }

    /// <summary>
    /// Your packing doesn't have any specifics on the dimensions or weight.
    /// </summary>
    private class YourPackagingType : FedExPackageType
    {
        public YourPackagingType() : base("YOUR_PACKAGING", 1)
        {
        }

        public override Dimensions Dimensions => new Dimensions(0m, 0m, 0m);

        public override decimal MaxWeight => 150m;

        public override decimal MinWeight => 0.1m;
    }

    /// <summary>
    /// FEDEX_PAK 50 lbs l: 12 1/4 (12.25)  w: 15 1/2 (15.50).
    /// </summary>
    private class FedExPakType : FedExPackageType
    {
        public FedExPakType() : base("FEDEX_PAK", 2)
        {
        }

        public override Dimensions Dimensions => new Dimensions(12m, 15m, 1.5m);

        public override decimal MaxWeight => 50m;

        public override decimal MinWeight => 1m;
    }

    // todo: add info
    private class FedEx10kgBoxType : FedExPackageType
    {
        public FedEx10kgBoxType() : base("FEDEX_10KG_BOX", 3)
        {
        }

        public override Dimensions Dimensions => new Dimensions(0m, 0m, 0m);

        public override decimal MaxWeight => 50m;

        public override decimal MinWeight => 1m;
    }

    // todo: add info
    private class FedEx25kgBoxType : FedExPackageType
    {
        public FedEx25kgBoxType() : base("FEDEX_25KG_BOX", 4)
        {
        }

        public override Dimensions Dimensions => new Dimensions(0m, 0m, 0m);

        public override decimal MaxWeight => 50m;

        public override decimal MinWeight => 1m;
    }

    // todo: add info
    private class FedExBoxType : FedExPackageType
    {
        public FedExBoxType() : base("FEDEX_BOX", 5)
        {
        }

        public override Dimensions Dimensions => new Dimensions(0m, 0m, 0m);

        public override decimal MaxWeight => 50m;

        public override decimal MinWeight => 1m;
    }

    // todo: add info
    private class FedExExtraLargeBoxType : FedExPackageType
    {
        public FedExExtraLargeBoxType() : base("FEDEX_EXTRA_LARGE_BOX", 6)
        {
        }

        public override Dimensions Dimensions => new Dimensions(0m, 0m, 0m);

        public override decimal MaxWeight => 50m;

        public override decimal MinWeight => 1m;
    }

    // todo: add info
    private class FedExLargeBoxType : FedExPackageType
    {
        public FedExLargeBoxType() : base("FEDEX_LARGE_BOX", 7)
        {
        }

        public override Dimensions Dimensions => new Dimensions(0m, 0m, 0m);

        public override decimal MaxWeight => 50m;

        public override decimal MinWeight => 1m;
    }

    // todo: add info
    private class FedExMediumBoxType : FedExPackageType
    {
        public FedExMediumBoxType() : base("FEDEX_MEDIUM_BOX", 8)
        {
        }

        public override Dimensions Dimensions => new Dimensions(0m, 0m, 0m);

        public override decimal MaxWeight => 50m;

        public override decimal MinWeight => 1m;
    }

    // todo: add info
    private class FedExSmallBoxType : FedExPackageType
    {
        public FedExSmallBoxType() : base("FEDEX_SMALL_BOX", 9)
        {
        }

        public override Dimensions Dimensions => new Dimensions(0m, 0m, 11m);

        public override decimal MaxWeight => 50m;

        public override decimal MinWeight => 1m;
    }

    // todo: add info
    private class FedExTubeType : FedExPackageType
    {
        public FedExTubeType() : base("FEDEX_TUBE", 10)
        {
        }

        public override Dimensions Dimensions => new Dimensions(0m, 0m, 0m);

        public override decimal MaxWeight => 50m;

        public override decimal MinWeight => 1m;
    }
}
