using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Usps.Rates;

public static class PackageExtensions
{
    public static bool IsPackageLarge(this Package package)
    {
        package = package ?? throw new ArgumentNullException(nameof(package));
        return package.IsOversize
            || package.Dimensions.Width > 12
            || package.Dimensions.Length > 12
            || package.Dimensions.Height > 12;
    }

    public static bool IsPackageMachinable(this Package package)
    {
        package = package ?? throw new ArgumentNullException(nameof(package));

        // Machinable parcels cannot be larger than 27 x 17 x 17 and cannot weight more than 25 lbs.
        if (package.Weight > 25)
        {
            return false;
        }

        return (package.Dimensions.Width <= 27 && package.Dimensions.Height <= 17 && package.Dimensions.Length <= 17)
            || (package.Dimensions.Width <= 17 && package.Dimensions.Height <= 27 && package.Dimensions.Length <= 17)
            || (package.Dimensions.Width <= 17 && package.Dimensions.Height <= 17 && package.Dimensions.Length <= 27);
    }

    /// <summary>
    /// https://pe.usps.com/BusinessMail101?ViewName=Flats
    /// Length  11-1/2 inches (11.50)   15 inches
    /// Height  6-1/8 inches (6.125)    12 inches
    /// Thickness   1/4 inch (0.25)     3/4 inch (0.75).
    /// </summary>
    /// <param name="package"></param>
    /// <returns></returns>
    public static bool IsEnvelope(this Package package)
    {
        package = package ?? throw new ArgumentNullException(nameof(package));

        return package.Dimensions.Length is >= 11.50m and <= 15m
            && package.Dimensions.Width is >= 6.125m and <= 12m
            && package.Dimensions.Height is >= 0.25m and <= 0.75m;
    }

    /// <summary>
    /// 15 oz or 0.9375 lbs.
    /// </summary>
    /// <param name="package"></param>
    /// <returns></returns>
    public static bool IsEnvelopeWeight(this Package package)
    {
        package = package ?? throw new ArgumentNullException(nameof(package));

        // 15 oz 0.9375
        return package.Weight <= 15m / 16;
    }

    /// <summary>
    /// https://www.stamps.com/usps/priority-mail-flat-rate/
    /// 8 11⁄16″ (8.6875) x 5 7⁄16″ (5.4375) x 1 3⁄4″ 1.75.
    /// </summary>
    /// <param name="package"></param>
    /// <returns></returns>
    public static bool IsSmallFlatRateBox(this Package package)
    {
        package = package ?? throw new ArgumentNullException(nameof(package));

        return package.Dimensions.Length <= 8m
            && package.Dimensions.Width <= 5m
            && package.Dimensions.Height <= 1m;
    }

    /// <summary>
    /// https://www.stamps.com/usps/priority-mail-flat-rate/
    /// 11 1⁄4″ (11.25) x 8 3⁄4″ (8.75) x 6″.
    /// </summary>
    /// <param name="package"></param>
    /// <returns></returns>
    public static bool IsMediumFlatRateBox(this Package package)
    {
        package = package ?? throw new ArgumentNullException(nameof(package));

        return package.Dimensions.Length <= 11m
            && package.Dimensions.Width <= 8m
            && package.Dimensions.Height <= 5m;
    }
}
