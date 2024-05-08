using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Rates.WebServices.Extensions;

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

        var packageType = FedExPackageType.FedExEnvelope;

        return package.Weight <= packageType.MaxWeight
                && package.Dimensions.Length <= package.Dimensions.Length
                && package.Dimensions.Width <= package.Dimensions.Width;
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

        var packageType = FedExPackageType.FedExPak;

        return package.Weight >= packageType.MinWeight
                && package.Weight <= packageType.MaxWeight
                && package.Dimensions.Length <= 12m
                && package.Dimensions.Width <= 15m;
    }

    /// <summary>
    /// This is an option for the shipments that are lesser than 1 lbs.
    /// </summary>
    /// <param name="package"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Null Exception thrown.</exception>
    public static bool IsFedExEnvelopeWeight(this Package package)
    {
        package = package ?? throw new ArgumentNullException(nameof(package));
        var packageType = FedExPackageType.FedExEnvelope;

        return package.Weight <= packageType.MaxWeight;
    }
}
