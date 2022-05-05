using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Rates.Extensions
{
    public static class PackageExtensions
    {
        public static bool IsLargeFlatEnvelope(this Package package)
        {
            package = package ?? throw new ArgumentNullException(nameof(package));

            return package.Weight < 0.8125m
                && package.Dimensions.Length <= 15m
                && package.Dimensions.Width <= 12m;
        }

        /// <summary>
        /// 12 1/2″ x 9 1/2″
        /// </summary>
        /// <param name="pacakge"></param>
        /// <returns></returns>
        public static bool IsPaddedFlatRateEnvelope(this Package package)
        {
            package = package ?? throw new ArgumentNullException(nameof(package));

            return package.Weight < 0.8125m &&
                package.Dimensions.Length <= 12.5m &&
                package.Dimensions.Width <= 9.5m &&
                package.Dimensions.Height <= 4m;
        }

        /// <summary>
        /// Small Flat Rate Box	USPS small flat rate box. A special 8-5/8” x 5-3/8” x 1-5/8” USPS box that clearly indicates “Small Flat Rate Box”.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsSmallFlatRateBox(this Package package)
        {
            package = package ?? throw new ArgumentNullException(nameof(package));

            return package.Dimensions.Length <= 8.625m &&
                package.Dimensions.Width <= 5.375m &&
                package.Dimensions.Height <= 1.625m;
        }

        /// <summary>
        /// Flat Rate Box	USPS medium flat rate box. A special 11” x 8 ½” x 5 ½” or 14” x 3.5” x 12” USPS box that clearly indicates “Medium Flat Rate Box”
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsFlatRateBox(this Package package)
        {
            package = package ?? throw new ArgumentNullException(nameof(package));

            return (package.Dimensions.Length <= 11m &&
                package.Dimensions.Width <= 8.5m &&
                package.Dimensions.Height <= 5.55m)
                ||
                (package.Dimensions.Length <= 14m &&
                package.Dimensions.Width <= 3.5m &&
                package.Dimensions.Height <= 12m);
        }

        /// <summary>
        /// Large Flat Rate Box	USPS large flat rate box. A special 12” x 12” x 6” USPS box that clearly indicates “Large Flat Rate Box”.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsLargeFlatRateBox(this Package package)
        {
            package = package ?? throw new ArgumentNullException(nameof(package));

            return package.Dimensions.Length <= 12m &&
                package.Dimensions.Width <= 12m &&
                package.Dimensions.Height <= 6m;
        }
    }
}
