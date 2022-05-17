using Ardalis.SmartEnum;

using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Abstractions.Models;

public abstract class PackageType : SmartEnum<PackageType>
{
    /// <summary>
    /// Postcard. Enabled on request.
    /// </summary>
    public static readonly PackageType PostCard = new PostCardType();

    /// <summary>
    /// Letter. Enabled on request.
    /// </summary>
    public static readonly PackageType Letter = new LetterType();

    /// <summary>
    /// Large envelope or flat. Has one dimension that is between 11 ½” and 15” long; 6 1/8” and 12” high; or ¼” and ¾ thick.
    /// </summary>
    public static readonly PackageType LargeEnvelopeOrFlat = new LargeEnvelopeOrFlatType();

    /// <summary>
    /// Thick envelope. Envelopes or flats greater than ¾” at the thickest point.
    /// </summary>
    public static readonly PackageType ThickEnvelope = new ThickEnvelopeType();

    /// <summary>
    /// Package. Longest side plus the distance around the thickest part is less than or equal to 84”.
    /// </summary>
    public static readonly PackageType Package = new Default();

    /// <summary>
    /// USPS small flat rate box. A special 8-5/8” x 5-3/8” x 1-5/8” USPS box that clearly indicates “Small Flat Rate Box”.
    /// </summary>
    public static readonly PackageType SmallFlatRateBox = new SmallFlatRateBoxType();

    /// <summary>
    /// USPS medium flat rate box. A special 11” x 8 ½” x 5 ½” or 14” x 3.5” x 12” USPS box that clearly indicates “Medium Flat Rate Box”.
    /// </summary>
    public static readonly PackageType FlatRateBox = new FlatRateBoxType();

    /// <summary>
    /// USPS large flat rate box. A special 12” x 12” x 6” USPS box that clearly indicates “Large Flat Rate Box”.
    /// </summary>
    public static readonly PackageType LargeFlatRateBox = new LargeFlatRateBoxType();

    /// <summary>
    /// USPS flat rate envelope. A special cardboard envelope provided by the USPS that clearly indicates “Flat Rate”.
    /// </summary>
    public static readonly PackageType FlatRateEnvelope = new FlatRateEnvelopeType();

    /// <summary>
    /// USPS flat rate padded envelope.
    /// </summary>
    public static readonly PackageType FlatRatePaddedEnvelope = new FlatRatePaddedEnvelopeType();

    /// <summary>
    /// Large package. Longest side plus the distance around the thickest part is over 84” and less than or equal to 108”.
    /// </summary>
    public static readonly PackageType LargePackage = new LargePackageType();

    /// <summary>
    /// Oversized package. Longest side plus the distance around the thickest part is over 108” and less than or equal to 130”.
    /// </summary>
    public static readonly PackageType OversizedPackage = new OversizedPackageType();

    /// <summary>
    /// USPS regional rate box A. A special 10 15/16” x 2 3/8” x 12 13/ 16” or 10” x 7” x 4 3/4” USPS box that clearly indicates “Regional Rate Box A”. 15 lbs maximum weight.
    /// </summary>
    public static readonly PackageType RegionalRateBoxA = new RegionalRateBoxAType();

    /// <summary>
    /// USPS regional rate box B. A special 14 3/8” x 2 2/8” x 15 7/8” or 12” x 10 1/4” x 5” USPS box that clearly indicates “Regional Rate Box B”. 20 lbs maximum weight.
    /// </summary>
    public static readonly PackageType RegionalRateBoxB = new RegionalRateBoxBType();

    /// <summary>
    /// USPS regional rate box C. A special 15” x 12” x 12” USPS box that clearly indicates ”Regional Rate Box C”. 25 lbs maximum weight.
    /// </summary>
    public static readonly PackageType RegionalRateBoxC = new RegionalRateBoxCType();

    /// <summary>
    /// USPS-supplied Priority Mail flat-rate envelope 9 1/2” x 15”. Maximum weight 4 pounds.
    /// </summary>
    public static readonly PackageType LegalFlatRateEnvelope = new LegalFlatRateEnvelopeType();

    /// <summary>
    /// This feature is not available for all integrations.
    /// </summary>
    public static readonly PackageType ExpressEnvelope = new ExpressEnvelopeType();

    /// <summary>
    /// This feature is not available for all integrations.
    /// </summary>
    public static readonly PackageType Documents = new DocumentType();

    /// <summary>
    /// This feature is not available for all integrations.
    /// </summary>
    public static readonly PackageType Envelope = new EnvelopeType();

    /// <summary>
    /// This feature is not available for all integrations.
    /// </summary>
    public static readonly PackageType Pak = new PakType();

    public PackageType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }

    public abstract Dimensions Dimensions { get; }

    public abstract decimal MaxWeight { get; }

    public abstract decimal MaxInternationalWeight { get; }

    public abstract decimal MinWeight { get; }

    /// <summary>
    /// Determined by the icons shown in the <b>service</b> dropdown button at <see href="https://print.testing.stamps.com/Webpostage/default2.aspx?"/>.
    /// </summary>
    public abstract string Category { get; }

    public string Description { get; }

    private sealed class PostCardType : PackageType
    {
        public PostCardType() : base(StampsClient.v111.PackageTypeV11.Postcard.ToString(), (int)StampsClient.v111.PackageTypeV11.Postcard, "Postcard Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(6m, 0.016m, 4.5m);

        public override decimal MaxWeight => .0625m;

        public override decimal MaxInternationalWeight => 4m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "PostCard";
    }

    private sealed class LetterType : PackageType
    {
        public LetterType() : base(StampsClient.v111.PackageTypeV11.Letter.ToString(), (int)StampsClient.v111.PackageTypeV11.Letter, "Letter Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(11.5m, 0.016m, 6.125m);

        public override decimal MaxWeight => .21875m;

        public override decimal MaxInternationalWeight => 4m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "Letter";
    }

    private sealed class LargeEnvelopeOrFlatType : PackageType
    {
        public LargeEnvelopeOrFlatType() : base(StampsClient.v111.PackageTypeV11.LargeEnvelopeorFlat.ToString(), (int)StampsClient.v111.PackageTypeV11.LargeEnvelopeorFlat, "Large Envelope Or Flat Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(15.00m, 0.75m, 12m);

        public override decimal MaxWeight => .8125m;

        public override decimal MaxInternationalWeight => 4.4m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "FlatRateEnvelope";
    }

    private sealed class ThickEnvelopeType : PackageType
    {
        public ThickEnvelopeType() : base(StampsClient.v111.PackageTypeV11.ThickEnvelope.ToString(), (int)StampsClient.v111.PackageTypeV11.ThickEnvelope, "Thick Envelope Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(22m, 15m, 15m);

        public override decimal MaxWeight => .999375m;

        public override decimal MaxInternationalWeight => 4m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "LargeEnvelope";
    }

    private sealed class SmallFlatRateBoxType : PackageType
    {
        public SmallFlatRateBoxType() : base(StampsClient.v111.PackageTypeV11.SmallFlatRateBox.ToString(), (int)StampsClient.v111.PackageTypeV11.SmallFlatRateBox, "Small Flat Rate Box Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(8.6875m, 5.375m, 1.75m);

        public override decimal MaxWeight => 70m;

        public override decimal MaxInternationalWeight => 4m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "FlatRateBox";
    }

    private sealed class Default : PackageType
    {
        public Default() : base(StampsClient.v111.PackageTypeV11.Package.ToString(), (int)StampsClient.v111.PackageTypeV11.Package, "Default Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(22m, 15m, 15m);

        public override decimal MaxWeight => 70m;

        public override decimal MaxInternationalWeight => 70m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "DefaultPackage";
    }

    private sealed class FlatRateBoxType : PackageType
    {
        public FlatRateBoxType() : base(StampsClient.v111.PackageTypeV11.FlatRateBox.ToString(), (int)StampsClient.v111.PackageTypeV11.FlatRateBox, "Flat Rate Box Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(14m, 3.5m, 12m);

        public override decimal MaxWeight => 70m;

        public override decimal MaxInternationalWeight => 20m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "FlatRateBox";
    }

    private sealed class FlatRatePaddedEnvelopeType : PackageType
    {
        public FlatRatePaddedEnvelopeType() : base(StampsClient.v111.PackageTypeV11.FlatRatePaddedEnvelope.ToString(), (int)StampsClient.v111.PackageTypeV11.FlatRatePaddedEnvelope, "Flat Rate Padded Envelope Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(12.5m, 0.75m, 9.5m);

        public override decimal MaxWeight => 70m;

        public override decimal MaxInternationalWeight => 4m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "FlatRateEnvelope";
    }

    private sealed class LargePackageType : PackageType
    {
        public LargePackageType() : base(StampsClient.v111.PackageTypeV11.LargePackage.ToString(), (int)StampsClient.v111.PackageTypeV11.LargePackage, "Large Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(24.0625m, 3.125m, 11.825m);

        public override decimal MaxWeight => 70m;

        public override decimal MaxInternationalWeight => 70m;


        public override decimal MinWeight => 0.001m;

        public override string Category => "DefaultPackage";
    }

    private sealed class OversizedPackageType : PackageType
    {
        public OversizedPackageType() : base(StampsClient.v111.PackageTypeV11.OversizedPackage.ToString(), (int)StampsClient.v111.PackageTypeV11.OversizedPackage, "Oversized Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(30m, 4m, 30m);

        public override decimal MaxWeight => 70m;

        public override decimal MaxInternationalWeight => 70m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "DefaultPackage";
    }

    private sealed class RegionalRateBoxAType : PackageType
    {
        public RegionalRateBoxAType() : base(StampsClient.v111.PackageTypeV11.RegionalRateBoxA.ToString(), (int)StampsClient.v111.PackageTypeV11.RegionalRateBoxA, "Regional Rate Box A Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(12.5m, 4m, 9.5m);

        public override decimal MaxWeight => 15m;

        public override decimal MaxInternationalWeight => 15m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "RegionalRateBox";
    }

    private sealed class RegionalRateBoxBType : PackageType
    {
        public RegionalRateBoxBType() : base(StampsClient.v111.PackageTypeV11.RegionalRateBoxB.ToString(), (int)StampsClient.v111.PackageTypeV11.RegionalRateBoxB, "Regional Rate Box B Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(16.25m, 3m, 14.5m);

        public override decimal MaxWeight => 20m;

        public override decimal MaxInternationalWeight => 20m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "RegionalRateBox";
    }

    private sealed class RegionalRateBoxCType : PackageType
    {
        public RegionalRateBoxCType() : base(StampsClient.v111.PackageTypeV11.RegionalRateBoxC.ToString(), (int)StampsClient.v111.PackageTypeV11.RegionalRateBoxC, "Regional Rate Box C Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(15m, 12m, 12m);

        public override decimal MaxWeight => 25m;

        public override decimal MaxInternationalWeight => 25m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "RegionalRateBox";
    }

    private sealed class LegalFlatRateEnvelopeType : PackageType
    {
        public LegalFlatRateEnvelopeType() : base(StampsClient.v111.PackageTypeV11.LegalFlatRateEnvelope.ToString(), (int)StampsClient.v111.PackageTypeV11.LegalFlatRateEnvelope, "Legal Flat Rate Envelope Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(15m, 0.75m, 9.5m);

        public override decimal MaxWeight => 4m;

        public override decimal MaxInternationalWeight => 4m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "FlatRateEnvelope";
    }

    /// <summary>
    /// Unkown.Enum selection is available from the wsdl but no documentation found on the type.
    /// </summary>
    private sealed class ExpressEnvelopeType : PackageType
    {
        public ExpressEnvelopeType() : base(StampsClient.v111.PackageTypeV11.ExpressEnvelope.ToString(), (int)StampsClient.v111.PackageTypeV11.ExpressEnvelope, "Express Envelope Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(1m, 1m, 1m);

        public override decimal MaxWeight => 4m;

        public override decimal MaxInternationalWeight => 4m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "Unknown";
    }

    /// <summary>
    /// Unkown.Enum selection is available from the wsdl but no documentation found on the type.
    /// </summary>
    private sealed class DocumentType : PackageType
    {
        public DocumentType() : base(StampsClient.v111.PackageTypeV11.Documents.ToString(), (int)StampsClient.v111.PackageTypeV11.Documents, "Documents Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(1m, 1m, 1m);

        public override decimal MaxWeight => 4m;

        public override decimal MaxInternationalWeight => 4m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "Unknown";
    }

    /// <summary>
    /// Unkown.Enum selection is available from the wsdl but no documentation found on the type.
    /// </summary>
    private sealed class EnvelopeType : PackageType
    {
        public EnvelopeType() : base(StampsClient.v111.PackageTypeV11.Envelope.ToString(), (int)StampsClient.v111.PackageTypeV11.Envelope, "Envelope Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(1m, 1m, 1m);

        public override decimal MaxWeight => 4m;

        public override decimal MaxInternationalWeight => 4m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "Unknown";
    }

    /// <summary>
    /// Unkown.Enum selection is available from the wsdl but no documentation found on the type.
    /// </summary>
    private sealed class PakType : PackageType
    {
        public PakType() : base(StampsClient.v111.PackageTypeV11.Pak.ToString(), (int)StampsClient.v111.PackageTypeV11.Pak, "Pak Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(1m, 1m, 1m);

        public override decimal MaxWeight => 4m;

        public override decimal MaxInternationalWeight => 4m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "Unknown";
    }

    private sealed class LargeFlatRateBoxType : PackageType
    {
        public LargeFlatRateBoxType() : base(StampsClient.v111.PackageTypeV11.LargeFlatRateBox.ToString(), (int)StampsClient.v111.PackageTypeV11.LargeFlatRateBox, "Large Flat Rate Box Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(24.0625m, 3.125m, 11.825m);

        public override decimal MaxWeight => 70m;

        public override decimal MaxInternationalWeight => 20m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "FlatRateBox";
    }

    private sealed class FlatRateEnvelopeType : PackageType
    {
        public FlatRateEnvelopeType() : base(StampsClient.v111.PackageTypeV11.FlatRateEnvelope.ToString(), (int)StampsClient.v111.PackageTypeV11.FlatRateEnvelope, "Flat Rate Envelope Package Type")
        {
        }

        public override Dimensions Dimensions => new Dimensions(9.5m, 0.75m, 12.5m);

        public override decimal MaxWeight => 70m;

        public override decimal MaxInternationalWeight => 4m;

        public override decimal MinWeight => 0.001m;

        public override string Category => "FlatRateEnvelope";
    }
}
