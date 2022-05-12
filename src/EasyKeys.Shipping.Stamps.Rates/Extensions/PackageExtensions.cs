using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Rates.Extensions
{
    public static class PackageExtensions
    {
        /// <summary>
        /// Flats and packages may not exceed 64 ounces, and packages cannot be more than 24 inches long and 36 inches in combined dimensions.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool DimensionsExceedFirstClassInternationalService(this Package package)
        {
            package = package ?? throw new ArgumentNullException(nameof(package));

            var totalDimensions = package.Dimensions.Length + package.Dimensions.Width + package.Dimensions.Height;

            return package.Weight > 4
                || package.Dimensions.Length > 24
                || package.Dimensions.Width > 24
                || package.Dimensions.Height > 24
                || totalDimensions > 36;
        }

        public static bool IsLargeEnvelope(this Package package)
        {
            package = package ?? throw new ArgumentNullException(nameof(package));

            return package.Weight <= 0.8125m
                && (package.Dimensions.Length > 11.5m && package.Dimensions.Length <= 15m)
                && (package.Dimensions.Height > 6.125m && package.Dimensions.Height <= 12m)
                && (package.Dimensions.Width <= .75m);
        }

        public static bool IsRegionalRateBoxA(this Package package)
        {
            package = package ?? throw new ArgumentNullException(nameof(package));

            return package.Weight < 15 &&
                ((package.Dimensions.Length <= 10.125m
                && package.Dimensions.Height <= 7.125m
                && package.Dimensions.Width <= 5m)
                ||
                (package.Dimensions.Height <= 13.0625m
                && package.Dimensions.Height <= 11.0625m
                && package.Dimensions.Width <= 2.5m));
        }

        public static bool IsRegionalRateBoxB(this Package package)
        {
            package = package ?? throw new ArgumentNullException(nameof(package));

            return package.Weight < 20 &&
                ((package.Dimensions.Length <= 12.25m
                && package.Dimensions.Height <= 10.5m
                && package.Dimensions.Width <= 5.5m)
                ||
                (package.Dimensions.Height <= 16.25m
                && package.Dimensions.Height <= 14.5m
                && package.Dimensions.Width <= 3m));
        }

        public static bool IsFlatRateEnvelope(this Package package)
        {
            package = package ?? throw new ArgumentNullException(nameof(package));

            return package.Weight <= 70m
                && package.Dimensions.Length < 9.5m
                && package.Dimensions.Height < 12.5m;
        }

        /// <summary>
        /// 12 1/2″ x 9 1/2″.
        /// </summary>
        /// <param name="pacakge"></param>
        /// <returns></returns>
        public static bool IsPaddedFlatRateEnvelope(this Package package)
        {
            package = package ?? throw new ArgumentNullException(nameof(package));

            return package.Weight < 70m &&
                ((package.Dimensions.Length <= 12.5m &&
                package.Dimensions.Height <= 9.5m) ||
                 (package.Dimensions.Height <= 12.5m &&
                package.Dimensions.Length <= 9.5m));
        }

        public static bool IsLegalFlatRateEnvelope(this Package package)
        {
            package = package ?? throw new ArgumentNullException(nameof(package));

            return package.Weight < 70m &&
                ((package.Dimensions.Length <= 15m &&
                package.Dimensions.Height <= 9.5m) ||
                 (package.Dimensions.Height <= 15m &&
                package.Dimensions.Length <= 9.5m));
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

            return package.Weight < 70m &&
                package.Dimensions.Length <= 8.6875m &&
                package.Dimensions.Height <= 5.375m &&
                package.Dimensions.Width <= 1.75m;
        }

        public static bool IsMediumFlatRateBox(this Package package)
        {
            package = package ?? throw new ArgumentNullException(nameof(package));

            return package.Weight < 70m &&
                ((package.Dimensions.Length <= 11.25m &&
                package.Dimensions.Height <= 8.75m &&
                package.Dimensions.Width <= 6m)
                ||
                (package.Dimensions.Length <= 14m &&
                package.Dimensions.Height <= 12m &&
                package.Dimensions.Width <= 3.5m));
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

            return package.Weight < 70m &&
                ((package.Dimensions.Length <= 12.25m &&
                package.Dimensions.Height <= 12.25m &&
                package.Dimensions.Width <= 6m)
                ||
                (package.Dimensions.Length <= 24.0625m &&
                package.Dimensions.Height <= 11.825m &&
                package.Dimensions.Width <= 3.125m));
        }
    }
}
