using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Rates.Extensions;

public static class PackageExtensions
{
    /// <summary>
    /// Flats and packages may not exceed 64 ounces, and packages cannot be more than 24 inches long and 36 inches in combined dimensions.
    /// </summary>
    /// <param name="package"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">null exeption for package.</exception>
    public static bool DimensionsExceedFirstClassInternationalService(this Package package)
    {
        package = package ?? throw new ArgumentNullException(nameof(package));

        var totalDimensions = package.Dimensions.Length + package.Dimensions.Width + package.Dimensions.Height;

        return package.Weight > 4.4m
            || package.Dimensions.Length > 24
            || package.Dimensions.Width > 24
            || package.Dimensions.Height > 24
            || totalDimensions > 36;
    }

    public static bool FitsPackageType(this Package package, PackageType packageType, bool isInternational = false)
    {
        return package.Dimensions.Length <= packageType.Dimensions.Length &&
               package.Dimensions.Width <= packageType.Dimensions.Width &&
               package.Dimensions.Height <= packageType.Dimensions.Height &&
               package.Weight <= (isInternational ? packageType.MaxInternationalWeight : packageType.MaxWeight);
    }
}
