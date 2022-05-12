using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates.Extensions;

namespace EasyKeys.Shipping.Stamps.Rates;

public class StampsRateConfigurator
{
    public StampsRateConfigurator(
        Address origin,
        Address destination,
        Package package,
        ContactInfo sender,
        ContactInfo receiver,
        DateTime? shipDate = null)
    {
        var solidDate = shipDate ?? DateTime.Now.AddDays(1);

        if (destination.IsUnitedStatesAddress())
        {
            // 1 lb = 16 oz => goes to first class
            if (package.Weight <= 0.999375m)
            {
                ConfigureDomesticFirstClass(
                                            origin,
                                            destination,
                                            package,
                                            sender,
                                            receiver,
                                            solidDate);
            }

            // 1 < weight < 70 => goes to priority shipping
            else
            {
                ConfigureDomesticPriority(
                                            origin,
                                            destination,
                                            package,
                                            sender,
                                            receiver,
                                            solidDate);
            }
        }
        else
        {

            if (package.DimensionsExceedFirstClassInternationalService())
            {
                ConfigureIntlPriority(
                                        origin,
                                        destination,
                                        package,
                                        sender,
                                        receiver,
                                        solidDate);
            }
            else
            {
                ConfigureIntlFirstClass(
                                            origin,
                                            destination,
                                            package,
                                            sender,
                                            receiver,
                                            solidDate);
            }
        }
    }

    public List<(Shipment shipment, RateRequestDetails rateOptions)> Shipments { get; } = new List<(Shipment shipment, RateRequestDetails rateOptions)>();

    /// <summary>
    /// Large envelope or flat. Has one dimension that is between 11 ½” and 15” long, 6 1/8” and 12” high, or ¼” and ¾ thick.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetLargeEnvelopeOrFlat(decimal weight)
    {
        return new Package(14.9m, 11.9m, 0m, weight, 0m);
    }

    /// <summary>
    /// Thick envelope. Envelopes or flats greater than ¾” at the thickest point.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetThickEnvelope(decimal weight)
    {
        return new Package(14.9m, 11.9m, 4m, weight, 0m);
    }

    /// <summary>
    /// Package. Longest side plus the distance around the thickest part is less than or equal to 84”.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetGenericPackage(decimal weight)
    {
        return new Package(40m, 40m, 4m, weight, 0m);
    }

    /// <summary>
    /// USPS small flat rate box. A special 8-5/8” x 5-3/8” x 1-5/8” USPS box that clearly indicates “Small Flat Rate Box”.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetSmallFlaxBox(decimal weight)
    {
        return new Package(8.625m, 5.375m, 1.625m, weight, 0m);
    }

    /// <summary>
    /// USPS medium flat rate box. A special 11” x 8 ½” x 5 ½” or 14” x 3.5” x 12” USPS box that clearly indicates “Medium Flat Rate Box”.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetFlaxBox(decimal weight)
    {
        return new Package(11m, 8.5m, 5.5m, weight, 0m);
    }

    /// <summary>
    /// USPS large flat rate box. A special 12” x 12” x 6” USPS box that clearly indicates “Large Flat Rate Box”.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetLargeFlaxBox(decimal weight)
    {
        return new Package(12m, 12m, 6m, weight, 0m);
    }

    /// <summary>
    /// USPS regional rate box A. A special 10 15/16” x 2 3/8” x 12 13/ 16” or 10” x 7” x 4 3/4” USPS box that clearly indicates “Regional Rate Box A”. 15 lbs maximum weight.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetRegionalRateBoxA(decimal weight)
    {
        return new Package(10m, 7m, 4.75m, weight, 0m);
    }

    /// <summary>
    /// USPS regional rate box B. A special 14 3/8” x 2 2/8” x 15 7/8” or 12” x 10 1/4” x 5” USPS box that clearly indicates “Regional Rate Box B”. 20 lbs maximum weight.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetRegionalRateBoxB(decimal weight)
    {
        return new Package(12m, 10.25m, 5m, weight, 0m);
    }

    /// <summary>
    /// USPS regional rate box C. A special 15” x 12” x 12” USPS box that clearly indicates ”Regional Rate Box C”. 25 lbs maximum weight.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetRegionalRateBoxC(decimal weight)
    {
        return new Package(15m, 12m, 12m, weight, 0m);
    }

    /// <summary>
    /// USPS-supplied Priority Mail flat-rate envelope 9 1/2” x 15”. Maximum weight 4 pounds.
    /// </summary>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static Package GetFlatRateEnvelope(decimal weight)
    {
        return new Package(9.5m, 15m, .7m, weight, 0m);
    }

    private void ConfigureDomesticFirstClass(
        Address origin,
        Address destination,
        Package package,
        ContactInfo sender,
        ContactInfo receiver,
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
            ServiceType = StampsServiceType.FirstClass,
        };

        var packageType = package.IsLargeFlatEnvelope() ? PackageType.LargeEnvelopeOrFlat : PackageType.Package;

        var shipmentOptions = new ShipmentOptions(packageType.Name, shipDate);

        var shipment = new Shipment(origin, destination, packages, shipmentOptions)
        {
            SenderInfo = sender,
            RecipientInfo = receiver
        };

        Shipments.Add((shipment, options));
    }

    private void ConfigureDomesticPriority(
        Address origin,
        Address destination,
        Package package,
        ContactInfo sender,
        ContactInfo receiver,
        DateTime shipDate)
    {
        if (package.Weight > 70)
        {
            throw new ArgumentException("Priority Mail package can't be greater than 70 lbs", nameof(package.Weight));
        }

        var packages = new List<Package> { package };

        var rateOptions = new RateRequestDetails()
        {
            ServiceType = StampsServiceType.Priority,
        };

        var packageType = GetFlatRatePackage(package);
        var shipOptions = new ShipmentOptions(packageType.Name, shipDate);

        var shipment = new Shipment(origin, destination, packages, shipOptions)
        {
            SenderInfo = sender,
            RecipientInfo = receiver
        };

        Shipments.Add((shipment, rateOptions));
    }

    private void ConfigureIntlFirstClass(
        Address origin,
        Address destination,
        Package package,
        ContactInfo sender,
        ContactInfo receiver,
        DateTime shipDate)
    {
        if (package.DimensionsExceedFirstClassInternationalService())
        {
            throw new ArgumentException("First Class Package International Service can't be greater than 4 lbs and and packages cannot be more than 24 inches long and 36 inches in combined dimensions", nameof(package.Weight));
        }

        var packages = new List<Package> { package };

        // will need to add commodity & customs information here.
        var rateOptions = new RateRequestDetails()
        {
            ServiceType = StampsServiceType.FirstClassInternational,
            DeclaredValue = package.InsuredValue,
            RegisteredValue = package.InsuredValue
        };

        var packageType = PackageType.Package;
        var shipOptions = new ShipmentOptions(packageType.Name, shipDate);

        var shipment = new Shipment(origin, destination, packages, shipOptions)
        {
            SenderInfo = sender,
            RecipientInfo = receiver
        };

        Shipments.Add((shipment, rateOptions));
    }

    private void ConfigureIntlPriority(
        Address origin,
        Address destination,
        Package package,
        ContactInfo sender,
        ContactInfo receiver,
        DateTime shipDate)
    {
        if (package.Weight > 70)
        {
            throw new ArgumentException("Priority Mail package can't be greater than 70 lbs", nameof(package.Weight));
        }

        var packages = new List<Package> { package };

        var packageType = GetFlatRatePackage(package);

        var shipOptions = new ShipmentOptions(packageType.Name, shipDate);

        // will need to add commodity & customs information here.
        var rateOptions = new RateRequestDetails()
        {
            ServiceType = StampsServiceType.PriorityInternational,
            DeclaredValue = package.InsuredValue,
            RegisteredValue = package.InsuredValue
        };

        var shipment = new Shipment(origin, destination, packages, shipOptions)
        {
            SenderInfo = sender,
            RecipientInfo = receiver
        };

        Shipments.Add((shipment, rateOptions));
    }

    private PackageType GetFlatRatePackage(Package package)
    {
        // sequence from smallest to largest
        if (package.IsPaddedFlatRateEnvelope())
        {
            return PackageType.FlatRatePaddedEnvelope;
        }

        if (package.IsSmallFlatRateBox())
        {
            return PackageType.SmallFlatRateBox;
        }

        if (package.IsFlatRateBox())
        {
            return PackageType.FlatRateBox;
        }

        if (package.IsLargeFlatRateBox())
        {
            return PackageType.LargeFlatRateBox;
        }

        return PackageType.Package;
    }
}
