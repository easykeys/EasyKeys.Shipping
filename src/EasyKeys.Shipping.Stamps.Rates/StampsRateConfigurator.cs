
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates.Extensions;

namespace EasyKeys.Shipping.Stamps.Rates
{
    public class StampsRateConfigurator
    {
        public StampsRateConfigurator(
            Address origin,
            Address destination,
            Package package,
            DateTime? shipDate = null)
        {
            var solidDate = shipDate ?? DateTime.Now.AddDays(1);

            if (destination.IsUnitedStatesAddress())
            {
                // 1 lb = 16 oz => goes to first class
                if (package.Weight <= 0.999375m)
                {
                    ConfigureDomesticFirstClass(origin, destination, package, solidDate);
                }

                // 1 < weight < 70 => goes to priority shipping
                else
                {
                    ConfigureDomesticPriority(origin, destination, package, solidDate);
                }
            }
            else
            {
                // international first class
                // international priority
            }
        }

        public List<(Shipment shipment, RateRequestDetails rateOptions)> Shipments { get; } = new List<(Shipment shipment, RateRequestDetails rateOptions)>();

        /// <summary>
        /// just holders for now
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static Package GetLargeFlatEnvelope(decimal weight)
        {
            return new Package(14.9m, 11.9m, 4m, weight, 0m);
        }

        public static Package GetFirstClassPackage(decimal weight)
        {
            return new Package(11m, 5m, 4m, weight, 0m);
        }

        public static Package GetSmallFlaxBox(decimal weight)
        {
            return new Package(11m, 5m, 4m, weight, 0m);
        }

        public static Package GetMediumFlatBox(decimal weight)
        {
            return new Package(11m, 5m, 4m, weight, 0m);
        }

        public static Package GetIntlEnvelope(decimal weight)
        {
            return new Package(11m, 5m, 4m, weight, 0m);
        }

        public static Package GetIntlBox(decimal weight)
        {
            return new Package(11m, 5m, 4m, weight, 0m);
        }

        private void ConfigureDomesticFirstClass(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate)
        {
            // 1 lb = 16 oz, must be less than 15.99
            if (package.Weight > 0.999375m)
            {
                throw new ArgumentException("First Class Mail package can't be greater than 15.99 oz", nameof(package.Weight));
            }

            var packages = new List<Package> { package };
            var options = new RateRequestDetails()
            {
                PackageType = package.IsLargeFlatEnvelope() ? PackageType.Large_Envelope_Or_Flat : PackageType.Package,
                ServiceDescription = "USPS First Class Mail",
                ServiceType = ServiceType.USPS_FIRST_CLASS_MAIL
            };
            var shipmentOptions = new ShipmentOptions()
            {
                ShippingDate = shipDate
            };

            var shipment = new Shipment(origin, destination, packages, shipmentOptions);

            Shipments.Add((shipment, options));
        }

        private void ConfigureDomesticPriority(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate)
        {
            if (package.Weight > 70)
            {
                throw new ArgumentException("Priority Mail package can't be greater than 70 lbs", nameof(package.Weight));
            }

            var packages = new List<Package> { package };

            var shipOptions = new ShipmentOptions
            {
                ShippingDate = shipDate
            };

            var rateOptions = new RateRequestDetails()
            {
                ServiceDescription = "USPS Priority Mail",
                ServiceType = ServiceType.USPS_PRIORITY_MAIL,
                PackageType = GetFlatRatePackage(package)
            };
        }

        private PackageType GetFlatRatePackage(Package package)
        {
            // sequence from smallest to largest
            if (package.IsPaddedFlatRateEnvelope())
            {
                return PackageType.Flat_Rate_Padded_Envelope;
            }

            if (package.IsSmallFlatRateBox())
            {
                return PackageType.Small_Flat_Rate_Box;
            }

            if (package.IsFlatRateBox())
            {
                return PackageType.Flat_Rate_Box;
            }

            if (package.IsLargeFlatRateBox())
            {
                return PackageType.Large_Flat_Rate_Box;
            }

            return PackageType.Package;
        }

        private object ConfigureLargePackage() => null;

        private object ConfigureDomesticSmallFlatRate() => null;

        private object ConfigureDomesticMediumFlatRate() => null;

        private object ConfigureIntlPriorityMail() => null;

        private object ConfigureIntlFirstClass() => null;
    }
}
