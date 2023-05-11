using System.Text;

using Ardalis.SmartEnum;

using EasyKeys.Shipping.Abstractions.Models;

using Humanizer;

namespace EasyKeys.Shipping.Stamps.Abstractions.Models;

/// <summary>
/// <para>Stamps.com USPS package type more information can be found at:</para>
/// <para>
///     <see href="https://pe.usps.com/text/dmm100/mailing-domestic.htm"/>.
///     <see href="https://pe.usps.com/text/dmm100/mailing-international.htm"/>.
/// </para>
/// </summary>
public abstract class StampsPackageType : SmartEnum<StampsPackageType>
{
    public static readonly StampsPackageType PostCard = new PostCardType(new Dimensions(5m, 3.25m, 0.007m), new Dimensions(6m, 4.25m, 0.016m));

    public static readonly StampsPackageType Letter = new LetterType(new Dimensions(5m, 3.25m, 0.0076m), new Dimensions(11.5m, 6.125m, 0.25m));

    public static readonly StampsPackageType LargeEnvelopeOrFlat = new LargeEnvelopeOrFlatType(new Dimensions(11.50m, 6.125m, 0.25m), new Dimensions(15m, 12m, 0.75m));

    public static readonly StampsPackageType ThickEnvelope = new ThickEnvelopeType(new Dimensions(6m, 3m, 0.25m), new Dimensions(22m, 15m, 0.75m));

    public static readonly StampsPackageType Package = new PackageType(new Dimensions(6m, 3m, 0.25m), new Dimensions(22m, 15m, 15m));

    public static readonly StampsPackageType LargePackage = new LargePackageType(new Dimensions(24.0625m, 11.825m, 3.125m), new Dimensions(24.0625m, 11.825m, 3.125m));

    /// <summary>
    /// <see href="https://store.usps.com/store/product/shipping-supplies/priority-mail-flat-rate-envelope-P_EP_14_F"/>.
    /// </summary>
    public static readonly StampsPackageType FlatRateEnvelope = new FlatRateEnvelopeType(new Dimensions(12.5m, 9.5m, 0.75m), new Dimensions(12.5m, 9.5m, 0.75m));

    /// <summary>
    /// <see href="https://store.usps.com/store/product/shipping-supplies/priority-mail-flat-rate-padded-envelope-p_ep14pe"/>.
    /// </summary>
    public static readonly StampsPackageType FlatRatePaddedEnvelope = new FlatRatePaddedEnvelopeType(new Dimensions(12.5m, 9.5m, 0.75m), new Dimensions(12.5m, 9.5m, 0.75m));

    /// <summary>
    /// <see href="https://store.stamps.com/products/priority-mail-small-flat-rate-box"/>.
    /// </summary>
    public static readonly StampsPackageType SmallFlatRateBox = new SmallFlatRateBoxType(new Dimensions(8.6875m, 5.475m, 1.75m), new Dimensions(8.6875m, 5.475m, 1.75m));

    /// <summary>
    /// <see href="https://store.usps.com/store/product/shipping-supplies/priority-mail-flat-rate-medium-box-1-p_o_frb1"/>.
    /// </summary>
    public static readonly StampsPackageType MediumFlatRateBox = new MediumFlatRateBoxType(new Dimensions(11.25m, 8.75m, 6m), new Dimensions(11.25m, 8.75m, 6m));

    public static readonly StampsPackageType LargeFlatRateBox = new LargeFlatRateBoxType(new Dimensions(12.25m, 12.25m, 6m), new Dimensions(12.25m, 12.25m, 6m));

    protected StampsPackageType(string name, int value) : base(name, value)
    {
    }

    public abstract Dimensions MinSize { get; protected set; }

    public abstract Dimensions MaxSize { get; protected set; }

    public abstract PackageWeight MaxWeight { get; }

    public abstract PackageWeight MaxInternationalWeight { get; }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendFormat(
            "{0} - Min W: {1} x L: {2} x H: {3} " +
            " Max W: {4} x L: {5} x H: {6}",
            Name.Humanize(LetterCasing.Title),
            MinSize.Width,
            MinSize.Length,
            MinSize.Height,
            MaxSize.Width,
            MaxSize.Length,
            MaxSize.Height);

        return builder.ToString();
    }

    private sealed class PostCardType : StampsPackageType
    {
        public PostCardType(
            Dimensions minSize,
            Dimensions maxSize)
            : base(
                  StampsClient.v111.PackageTypeV11.Postcard.ToString(),
                  (int)StampsClient.v111.PackageTypeV11.Postcard)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public override Dimensions MinSize { get; protected set; }

        public override Dimensions MaxSize { get; protected set; }

        public override PackageWeight MaxWeight => new PackageWeight(0.11m, isOunce: true);


        public override PackageWeight MaxInternationalWeight => MaxWeight;
    }

    private sealed class LetterType : StampsPackageType
    {
        public LetterType(
            Dimensions minSize,
            Dimensions maxSize)
            : base(
                  StampsClient.v111.PackageTypeV11.Letter.ToString(),
                  (int)StampsClient.v111.PackageTypeV11.Letter)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public override Dimensions MaxSize { get; protected set; }

        public override Dimensions MinSize { get; protected set; }

        public override PackageWeight MaxWeight => new PackageWeight(3.5m, isOunce: true);

        public override PackageWeight MaxInternationalWeight => MaxWeight;
    }

    private sealed class LargeEnvelopeOrFlatType : StampsPackageType
    {
        public LargeEnvelopeOrFlatType(
            Dimensions minSize,
            Dimensions maxSize)
            : base(
                  StampsClient.v111.PackageTypeV11.LargeEnvelopeorFlat.ToString(),
                  (int)StampsClient.v111.PackageTypeV11.LargeEnvelopeorFlat)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public override Dimensions MaxSize { get; protected set; }

        public override Dimensions MinSize { get; protected set; }

        public override PackageWeight MaxWeight => new PackageWeight(13m, isOunce: true);

        public override PackageWeight MaxInternationalWeight => MaxWeight;
    }

    private sealed class ThickEnvelopeType : StampsPackageType
    {
        public ThickEnvelopeType(
            Dimensions minSize,
            Dimensions maxSize)
            : base(
                  StampsClient.v111.PackageTypeV11.ThickEnvelope.ToString(),
                  (int)StampsClient.v111.PackageTypeV11.ThickEnvelope)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public override Dimensions MaxSize { get; protected set; }

        public override Dimensions MinSize { get; protected set; }

        public override PackageWeight MaxWeight => new PackageWeight(15.99m, isOunce: true);

        public override PackageWeight MaxInternationalWeight => MaxWeight;
    }

    private sealed class PackageType : StampsPackageType
    {
        public PackageType(
            Dimensions minSize,
            Dimensions maxSize)
            : base(
                  StampsClient.v111.PackageTypeV11.Package.ToString(),
                  (int)StampsClient.v111.PackageTypeV11.Package)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public override Dimensions MaxSize { get; protected set; }

        public override Dimensions MinSize { get; protected set; }

        public override PackageWeight MaxWeight => new PackageWeight(70m, isOunce: false);

        public override PackageWeight MaxInternationalWeight => MaxWeight;
    }

    private sealed class LargePackageType : StampsPackageType
    {
        public LargePackageType(
            Dimensions minSize,
            Dimensions maxSize)
            : base(
                  StampsClient.v111.PackageTypeV11.LargePackage.ToString(),
                  (int)StampsClient.v111.PackageTypeV11.LargePackage)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public override Dimensions MaxSize { get; protected set; }

        public override Dimensions MinSize { get; protected set; }

        public override PackageWeight MaxWeight => new PackageWeight(70m, isOunce: false);

        public override PackageWeight MaxInternationalWeight => new PackageWeight(4m, isOunce: false);
    }

    private sealed class FlatRateEnvelopeType : StampsPackageType
    {
        public FlatRateEnvelopeType(
            Dimensions minSize,
            Dimensions maxSize)
            : base(
                  StampsClient.v111.PackageTypeV11.FlatRateEnvelope.ToString(),
                  (int)StampsClient.v111.PackageTypeV11.FlatRateEnvelope)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public override Dimensions MaxSize { get; protected set; }

        public override Dimensions MinSize { get; protected set; }

        public override PackageWeight MaxWeight => new PackageWeight(70m, isOunce: false);

        public override PackageWeight MaxInternationalWeight => new PackageWeight(4m, isOunce: false);
    }

    private sealed class FlatRatePaddedEnvelopeType : StampsPackageType
    {
        public FlatRatePaddedEnvelopeType(
            Dimensions minSize,
            Dimensions maxSize)
            : base(
                  StampsClient.v111.PackageTypeV11.FlatRatePaddedEnvelope.ToString(),
                  (int)StampsClient.v111.PackageTypeV11.FlatRatePaddedEnvelope)
        {
        }

        public override Dimensions MaxSize { get; protected set; }

        public override Dimensions MinSize { get; protected set; }

        public override PackageWeight MaxWeight => new PackageWeight(70m, isOunce: false);

        public override PackageWeight MaxInternationalWeight => new PackageWeight(4m, isOunce: false);
    }

    private sealed class SmallFlatRateBoxType : StampsPackageType
    {
        public SmallFlatRateBoxType(
            Dimensions minSize,
            Dimensions maxSize)
            : base(
                  StampsClient.v111.PackageTypeV11.SmallFlatRateBox.ToString(),
                  (int)StampsClient.v111.PackageTypeV11.SmallFlatRateBox)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public override Dimensions MaxSize { get; protected set; }

        public override Dimensions MinSize { get; protected set; }

        public override PackageWeight MaxWeight => new PackageWeight(70m, isOunce: false);

        public override PackageWeight MaxInternationalWeight => new PackageWeight(4m, isOunce: false);
    }

    private sealed class MediumFlatRateBoxType : StampsPackageType
    {
        public MediumFlatRateBoxType(
            Dimensions minSize,
            Dimensions maxSize)
            : base(
                StampsClient.v111.PackageTypeV11.FlatRateBox.ToString(),
                (int)StampsClient.v111.PackageTypeV11.FlatRateBox)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public override Dimensions MaxSize { get; protected set; }

        public override Dimensions MinSize { get; protected set; }

        public override PackageWeight MaxWeight => new PackageWeight(70m, isOunce: false);

        public override PackageWeight MaxInternationalWeight => new PackageWeight(20m, isOunce: false);
    }

    private sealed class LargeFlatRateBoxType : StampsPackageType
    {
        public LargeFlatRateBoxType(
            Dimensions minSize,
            Dimensions maxSize)
            : base(
                  StampsClient.v111.PackageTypeV11.LargeFlatRateBox.ToString(),
                  (int)StampsClient.v111.PackageTypeV11.LargeFlatRateBox)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public override Dimensions MinSize { get; protected set; }

        public override Dimensions MaxSize { get; protected set; }

        public override PackageWeight MaxWeight => new PackageWeight(70m, isOunce: false);

        public override PackageWeight MaxInternationalWeight => new PackageWeight(20m, isOunce: false);
    }
}
