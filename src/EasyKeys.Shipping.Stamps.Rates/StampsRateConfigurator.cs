using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates.Extensions;

namespace EasyKeys.Shipping.Stamps.Rates;

/// <summary>
/// Generates a list of shipments and rate details.
/// These shipments are composed of the smallest and best priced packaging type for the service requested.
/// </summary>
public class StampsRateConfigurator
{
    public StampsRateConfigurator(
        Address origin,
        Address destination,
        Package package,
        DateTime? shipDate = null)
    {
        // helpful to make sure we have business days and not regular days.
        var solidDate = shipDate ?? DateTime.Now;

        // configure possible shipments
        if (destination.IsUnitedStatesAddress())
        {
            CreateDomesticShipments(origin, destination, package, solidDate);
        }
        else
        {
            CreateInternationalShipments(origin, destination, package, solidDate);
        }
    }

    public List<Shipment> Shipments { get; } = new List<Shipment>();

    /// <summary>
    /// Large envelope or flat. Has one dimension that is between 11 ½” and 15” long, 6 1/8” and 12” high, or ¼” and ¾ thick.
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    /// <returns></returns>
    public static Package GetLargeEnvelopeOrFlat(
        decimal weight,
        decimal insuredValue = 20,
        bool isSignatureRequired = false)
    {
        return new Package(StampsPackageType.LargeEnvelopeOrFlat.Dimensions, weight, insuredValue, isSignatureRequired);
    }

    /// <summary>
    /// Thick envelope. Envelopes or flats greater than ¾” at the thickest point.
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    /// <returns></returns>
    public static Package GetThickEnvelope(
        decimal weight,
        decimal insuredValue = 20,
        bool isSignatureRequired = false)
    {
        return new Package(StampsPackageType.ThickEnvelope.Dimensions, weight, insuredValue, isSignatureRequired);
    }

    /// <summary>
    /// Package. Longest side plus the distance around the thickest part is less than or equal to 84”.
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    /// <returns></returns>
    public static Package GetGenericPackage(
        decimal weight,
        decimal insuredValue = 20,
        bool isSignatureRequired = false)
    {
        return new Package(StampsPackageType.Package.Dimensions, weight, insuredValue, isSignatureRequired);
    }

    /// <summary>
    /// USPS small flat rate box. A special 8-5/8” x 5-3/8” x 1-5/8” USPS box that clearly indicates “Small Flat Rate Box”.
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    /// <returns></returns>
    public static Package GetSmallFlaxBox(
        decimal weight,
        decimal insuredValue = 20,
        bool isSignatureRequired = false)
    {
        return new Package(StampsPackageType.SmallFlatRateBox.Dimensions, weight, insuredValue, isSignatureRequired);
    }

    /// <summary>
    /// USPS medium flat rate box. A special 11” x 8 ½” x 5 ½” or 14” x 3.5” x 12” USPS box that clearly indicates “Medium Flat Rate Box”.
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    /// <returns></returns>
    public static Package GetFlaxBox(
        decimal weight,
        decimal insuredValue = 20,
        bool isSignatureRequired = false)
    {
        return new Package(StampsPackageType.FlatRateBox.Dimensions, weight, insuredValue, isSignatureRequired);
    }

    /// <summary>
    /// USPS large flat rate box. A special 12” x 12” x 6” USPS box that clearly indicates “Large Flat Rate Box”.
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    /// <returns></returns>
    public static Package GetLargeFlaxBox(
        decimal weight,
        decimal insuredValue = 20,
        bool isSignatureRequired = false)
    {
        return new Package(StampsPackageType.LargeFlatRateBox.Dimensions, weight, insuredValue, isSignatureRequired);
    }

    /// <summary>
    /// USPS regional rate box A. A special 10 15/16” x 2 3/8” x 12 13/ 16” or 10” x 7” x 4 3/4” USPS box that clearly indicates “Regional Rate Box A”. 15 lbs maximum weight.
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    /// <returns></returns>
    public static Package GetRegionalRateBoxA(
        decimal weight,
        decimal insuredValue = 20,
        bool isSignatureRequired = false)
    {
        return new Package(StampsPackageType.RegionalRateBoxA.Dimensions, weight, insuredValue, isSignatureRequired);
    }

    /// <summary>
    /// USPS regional rate box B. A special 14 3/8” x 2 2/8” x 15 7/8” or 12” x 10 1/4” x 5” USPS box that clearly indicates “Regional Rate Box B”. 20 lbs maximum weight.
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    /// <returns></returns>
    public static Package GetRegionalRateBoxB(
        decimal weight,
        decimal insuredValue = 20,
        bool isSignatureRequired = false)
    {
        return new Package(StampsPackageType.RegionalRateBoxB.Dimensions, weight, insuredValue, isSignatureRequired);
    }

    /// <summary>
    /// USPS regional rate box C. A special 15” x 12” x 12” USPS box that clearly indicates ”Regional Rate Box C”. 25 lbs maximum weight.
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    /// <returns></returns>
    public static Package GetRegionalRateBoxC(
        decimal weight,
        decimal insuredValue = 20,
        bool isSignatureRequired = false)
    {
        return new Package(StampsPackageType.RegionalRateBoxC.Dimensions, weight, insuredValue, isSignatureRequired);
    }

    /// <summary>
    /// USPS-supplied Priority Mail flat-rate envelope 9 1/2” x 15”. Maximum weight 4 pounds.
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    /// <returns></returns>
    public static Package GetFlatRateEnvelope(
        decimal weight,
        decimal insuredValue = 20,
        bool isSignatureRequired = false)
    {
        return new Package(StampsPackageType.FlatRateEnvelope.Dimensions, weight, insuredValue, isSignatureRequired);
    }

    private void CreateInternationalShipments(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate)
    {
        if (package.DimensionsExceedFirstClassInternationalService())
        {
            ConfigureIntlPriority(origin, destination, package, shipDate);
        }
        else
        {
            ConfigureIntlFirstClass(origin, destination, package, shipDate);
            ConfigureIntlPriority(origin, destination, package, shipDate);
        }
    }

    private void CreateDomesticShipments(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate)
    {
        // 1 lb = 16 oz, must be less than 15.99
        if (package.Weight > 0.999375m)
        {
            ConfigureDomesticPriority(origin, destination, package, shipDate);
        }
        else
        {
            ConfigureDomesticFirstClass(origin, destination, package, shipDate);
            ConfigureDomesticPriority(origin, destination, package, shipDate);
        }
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

        var packageTypes = StampsPackageType.List.Where(x => x.Category == "DefaultPackage" || x.Category == "LargeEnvelope");

        foreach (var packageType in packageTypes)
        {
            if (package.FitsPackageType(packageType))
            {
                var shipmentOptions = new ShipmentOptions(packageType.Name, shipDate);

                var shipment = new Shipment(origin, destination, packages, shipmentOptions);

                Shipments.Add(shipment);
            }
        }
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

        var packageTypes = StampsPackageType.List.Where(x => x.Category != "Unknown"
                            && x.Category != "Letter"
                            && x.Category != "PostCard");

        foreach (var packageType in packageTypes)
        {
            if (package.FitsPackageType(packageType))
            {
                var shipOptions = new ShipmentOptions(packageType.Name, shipDate);

                var shipment = new Shipment(origin, destination, packages, shipOptions);

                if (Shipments.Any(x => x.Options.PackagingType == packageType.Name))
                {
                    return;
                }

                Shipments.Add(shipment);
            }
        }
    }

    private void ConfigureIntlFirstClass(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate)
    {
        if (package.DimensionsExceedFirstClassInternationalService())
        {
            throw new ArgumentException("First Class Package International Service can't be greater than 4.4 lbs and and packages cannot be more than 24 inches long and 36 inches in combined dimensions", nameof(package.Weight));
        }

        var packages = new List<Package> { package };

        var packageTypes = StampsPackageType.List
            .Where(x => x.Category == "DefaultPackage" || x.Category == "LargeEnvelope");

        foreach (var packageType in packageTypes)
        {
            if (package.FitsPackageType(packageType, isInternational: true))
            {
                var shipmentOptions = new ShipmentOptions(packageType.Name, shipDate);

                var shipment = new Shipment(origin, destination, packages, shipmentOptions);

                Shipments.Add(shipment);
            }
        }
    }

    private void ConfigureIntlPriority(
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

        var packageTypes = StampsPackageType.List.Where(x => x.Category != "Unknown"
                    && x.Category != "Letter"
                    && x.Category == "PostCard");

        foreach (var packageType in packageTypes)
        {
            if (package.FitsPackageType(packageType, isInternational: true))
            {
                var shipOptions = new ShipmentOptions(packageType.Name, shipDate);

                var shipment = new Shipment(origin, destination, packages, shipOptions);

                if (Shipments.Any(x => x.Options.PackagingType == packageType.Name))
                {
                    return;
                }

                Shipments.Add(shipment);
            }
        }
    }
}
