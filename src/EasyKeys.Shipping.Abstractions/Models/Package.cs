namespace EasyKeys.Shipping.Abstractions.Models;

/// <summary>
/// The class definition for actual material shipping package.
/// </summary>
public class Package : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Package"/> class.
    /// Creates a new Shipment package.
    /// </summary>
    /// <param name="dimensions">The dimensions of the package, in inches.</param>
    /// <param name="weight">The weight of the package, in pounds.</param>
    /// <param name="insuredValue"></param>
    /// <param name="signatureRequiredOnDelivery"></param>
    public Package(
        Dimensions dimensions,
        decimal weight,
        decimal insuredValue,
        bool signatureRequiredOnDelivery)
    {
        Dimensions = dimensions;
        Weight = weight;
        InsuredValue = insuredValue;
        SignatureRequiredOnDelivery = signatureRequiredOnDelivery;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Package"/> class.
    /// Creates a new package with 0 insurance value and signature is not required.
    /// </summary>
    /// <param name="dimensions"></param>
    /// <param name="weight"></param>
    public Package(
        Dimensions dimensions,
        decimal weight)
        : this(dimensions, weight, 0m, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Package"/> class.
    ///     Creates a new package object.
    /// </summary>
    /// <param name="length">The length of the package, in inches.</param>
    /// <param name="width">The width of the package, in inches.</param>
    /// <param name="height">The height of the package, in inches.</param>
    /// <param name="weight">The weight of the package, in pounds.</param>
    /// <param name="insuredValue">The insured-value of the package, in dollars.</param>
    /// <param name="signatureRequiredOnDelivery">If true, will attempt to send this to the appropriate rate provider.</param>
    public Package(
        int length,
        int width,
        int height,
        int weight,
        decimal insuredValue,
        bool signatureRequiredOnDelivery = false)
        : this(length, width, height, (decimal)weight, insuredValue, signatureRequiredOnDelivery)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Package"/> class.
    ///     Creates a new package object.
    /// </summary>
    /// <param name="length">The length of the package, in inches.</param>
    /// <param name="width">The width of the package, in inches.</param>
    /// <param name="height">The height of the package, in inches.</param>
    /// <param name="weight">The weight of the package, in pounds.</param>
    /// <param name="insuredValue">The insured-value of the package, in dollars.</param>
    /// <param name="signatureRequiredOnDelivery">If true, will attempt to send this to the appropriate rate provider.</param>
    public Package(
        decimal length,
        decimal width,
        decimal height,
        decimal weight,
        decimal insuredValue,
        bool signatureRequiredOnDelivery = false)
        : this(new Dimensions(length, width, height), weight, insuredValue, signatureRequiredOnDelivery)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Package"/> class.
    /// Required for json serialization.
    /// </summary>
    public Package()
    {
    }

    public Dimensions Dimensions { get; set; }

    public decimal InsuredValue { get; set; } = 20;

    public bool IsOversize { get; set; }

    public decimal Weight { get; set; }

    public decimal RoundedWeight => Math.Ceiling(Weight);

    public bool SignatureRequiredOnDelivery { get; set; }

    public PoundsAndOunces PoundsAndOunces => new PackageWeight(Weight, isOunce: false).PoundsAndOunces;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Dimensions;
        yield return InsuredValue;
        yield return IsOversize;
        yield return Weight;
        yield return SignatureRequiredOnDelivery;
    }
}
