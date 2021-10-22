using EasyKeys.Shipping.Abstractions;

namespace EasyKeys.Shipping.FedEx;

public static class PackageExtensions
{
    /// <summary>
    /// The main rule is that weight of the package must be below 1 lbs.
    /// FEDEX_ENVELOPE 10 lbs l: 9 1/2 (9.50) x w:12 1/2 (12.50).
    /// </summary>
    /// <param name="package"></param>
    /// <returns></returns>
    public static bool IsFedExEnvelope(this Package package)
    {
        package = package ?? throw new ArgumentNullException(nameof(package));

        return package.Weight <= 1m
                && package.Dimensions.Length <= 9m
                && package.Dimensions.Width <= 12m;
    }

    /// <summary>
    /// This is an option for the shipments that are greater than 1 lbs.
    /// FEDEX_PAK 50 lbs l: 12 1/4 (12.25)  w: 15 1/2 (15.50).
    /// </summary>
    /// <param name="package"></param>
    /// <returns></returns>
    public static bool IsFedExPak(this Package package)
    {
        package = package ?? throw new ArgumentNullException(nameof(package));

        return package.Weight is > 1m and <= 50
                && package.Dimensions.Length <= 12m
                && package.Dimensions.Width <= 15m;
    }

    public static bool IsFedExEnvelopeWeight(this Package package)
    {
        package = package ?? throw new ArgumentNullException(nameof(package));
        return package.Weight <= 1m;
    }
}
