using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Usps.Rates;

public class UspsRateConfigurator
{
    public UspsRateConfigurator(
        Address origin,
        Address destination,
        Package package,
        DateTime? shipDate = null)
    {
        // since USPS is not highest priority it is okay to add some time for processing it.
        var solidDate = shipDate ?? DateTime.Now.AddDays(1);

        if (destination.IsUnitedStatesAddress())
        {
            // weight > 15 oz
            if (package.Weight <= 15m / 16)
            {
                ConfigureDomesticFirstClass(origin, destination, package, solidDate);
                ConfigureDomesticSmallFlatRate(origin, destination, GetSmallFlatBox(package.Weight), solidDate);
            }
            else
            {
                ConfigureDomesticPriority(origin, destination, package, solidDate);
            }
        }
        else
        {
            ConfigureIntlFirstClass(origin, destination, package, solidDate);
            ConfigureIntlPriorityMail(origin, destination, package, solidDate);
        }
    }

    public List<(Shipment shipment, UspsRateOptions rateOptions)> Shipments { get; } = new List<(Shipment shipment, UspsRateOptions rateOptions)>();

    public decimal StampsRate { get; private set; }

    public static Package GetPackage(
        Address destination,
        decimal weight,
        Dimensions size)
    {
        var package = new Package(size, weight);

        if (destination.IsUnitedStatesAddress())
        {
            if (package.Weight <= 15m / 16)
            {
                return GetFirstClassPackage(package.Weight);
            }
            else
            {
                if (package.IsSmallFlatRateBox())
                {
                    return GetSmallFlatBox(package.Weight);
                }

                if (package.IsMediumFlatRateBox())
                {
                    return GetMediumFlatBox(package.Weight);
                }
            }
        }
        else
        {
            return GetIntlBox(package.Weight);
        }

        return package;
    }

    /// <summary>
    /// Create First Class package weight 0 - 15 oz.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetFirstClassPackage(decimal weight)
    {
        if (weight > 15m / 16)
        {
            throw new ArgumentException("Exceeds first class weight", nameof(weight));
        }

        var ozW = weight * 16;
        if (ozW >= 13m || ozW >= 15m)
        {
            weight = 13m / 16;
        }

        return new Package(11m, 6m, 0.75m, weight, 0m);
    }

    /// <summary>
    /// https://store.usps.com/store/product/shipping-supplies/priority-mail-small-flat-rate-box-P_SMALL_FRB
    /// Small Flat Rate Box up to 70 lbs.   8 11⁄16″ x 5 7⁄16″ x 1 3⁄4″ usps: $8.45  stamps.com: $7.90
    /// https://www.stamps.com/usps/priority-mail-flat-rate/.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetSmallFlatBox(decimal weight)
    {
        return new Package(8m, 5m, 1m, weight, 0m);
    }

    /// <summary>
    /// Medium Flat Rate Box - Top Loading   up to 70 lbs.   11 1⁄4″ x 8 3⁄4″ x 6″  usps: $15.50    stamps: $13.75
    /// https://www.stamps.com/usps/priority-mail-flat-rate/.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetMediumFlatBox(decimal weight)
    {
        return new Package(11m, 8m, 5m, weight, 0m);
    }

    /// <summary>
    /// First Class International ® $7.90 Large Envelope or Flat.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetIntlEnvelope(decimal weight)
    {
        if (weight > 4m)
        {
            throw new ArgumentException("Exceeds first class weight", nameof(weight));
        }

        return new Package(11m, 6m, 0.75m, weight, 0m);
    }

    /// <summary>
    /// https://www.usps.com/international/priority-mail-international.htm
    /// Maximum weight for Priority Mail International Flat Rate Envelopes and small Flat Rate Boxes is 4 lbs.
    /// Fast international delivery time: 6–10 business days.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetIntlBox(decimal weight)
    {
        return new Package(8m, 5m, 1m, weight, 0m);
    }

    private void ConfigureDomesticFirstClass(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate)
    {
        if (package.Weight > 15m / 16)
        {
            throw new ArgumentException("First Class Mail package can't be greater than 15 oz", nameof(package.Weight));
        }

        // domestic first class package
        var packages = new List<Package> { package };
        var shipOptions = new ShipmentOptions(nameof(FirstClassPackageType.PACKAGE_SERVICE_RETAIL), shipDate);

        var shipment = new Shipment(origin, destination, packages, shipOptions);
        var rateOptions = new UspsRateOptions
        {
            ServiceName = "First Class",
            DefaultGuaranteedDelivery = shipDate.AddBusinessDays(5)
        };

        Shipments.Add((shipment, rateOptions));
    }

    private void ConfigureDomesticPriority(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate)
    {
        if (package.Weight > 70m)
        {
            throw new ArgumentException("Priority Mail package can't be greater than 70 lbs", nameof(package.Weight));
        }

        if (package.IsSmallFlatRateBox())
        {
            ConfigureDomesticSmallFlatRate(origin, destination, package, shipDate);
            return;
        }

        if (package.IsMediumFlatRateBox())
        {
            ConfigureDomesticMediumFlatRate(origin, destination, package, shipDate);
            return;
        }

        ConfigureLargePackage(origin, destination, package, shipDate);
    }

    private void ConfigureLargePackage(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate)
    {
        var packages = new List<Package> { package };

        var shipOptions = new ShipmentOptions(nameof(PriorityPackageType.VARIABLE), shipDate);

        var shipment = new Shipment(origin, destination, packages, shipOptions);
        var rateOptions = new UspsRateOptions
        {
            ServiceName = "Priority",
            DefaultGuaranteedDelivery = shipDate.AddBusinessDays(3)
        };

        Shipments.Add((shipment, rateOptions));
    }

    private void ConfigureDomesticSmallFlatRate(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate)
    {
        var packages = new List<Package> { package };

        var shipOptions = new ShipmentOptions(nameof(PriorityPackageType.SM_FLAT_RATE_BOX), shipDate);

        StampsRate = 7.90m;

        var shipment = new Shipment(origin, destination, packages, shipOptions);
        var rateOptions = new UspsRateOptions
        {
            ServiceName = "Priority",
            DefaultGuaranteedDelivery = shipDate.AddBusinessDays(3)
        };

        Shipments.Add((shipment, rateOptions));
    }

    private void ConfigureDomesticMediumFlatRate(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate)
    {
        var packages = new List<Package> { package };
        var shipOptions = new ShipmentOptions(nameof(PriorityPackageType.MD_FLAT_RATE_BOX), shipDate);

        StampsRate = 13.75m;

        var shipment = new Shipment(origin, destination, packages, shipOptions);
        var rateOptions = new UspsRateOptions
        {
            ServiceName = "Priority",
            DefaultGuaranteedDelivery = shipDate.AddBusinessDays(3)
        };

        Shipments.Add((shipment, rateOptions));
    }

    private void ConfigureIntlPriorityMail(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate)
    {
        var packages = new List<Package> { package };

        var shipOptions = new ShipmentOptions(nameof(IntlPackageType.PACKAGE), shipDate);

        var shipment = new Shipment(origin, destination, packages, shipOptions);

        // var rateOptions = new UspsRateOptions { ServiceName = "All" };
        var rateOptions = new UspsRateOptions
        {
            ServiceName = "Priority Mail International",
            DefaultGuaranteedDelivery = shipDate.AddBusinessDays(14)
        };

        Shipments.Add((shipment, rateOptions));
    }

    private void ConfigureIntlFirstClass(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate)
    {
        if (package.Weight > 4m)
        {
            throw new ArgumentException("Package can't be greater than 4 lbs", nameof(package.Weight));
        }

        var packages = new List<Package> { package };

        var shipOptions = new ShipmentOptions(nameof(IntlPackageType.ALL), shipDate);

        StampsRate = 7.90m;

        var shipment = new Shipment(origin, destination, packages, shipOptions);
        var rateOptions = new UspsRateOptions
        {
            ServiceName = "First-Class Package International Service",
            DefaultGuaranteedDelivery = shipDate.AddBusinessDays(21)
        };

        Shipments.Add((shipment, rateOptions));
    }
}
