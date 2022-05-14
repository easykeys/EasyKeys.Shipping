using EasyKeys.Shipping.Abstractions;
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
        StampsServiceType? serviceType = null,
        DateTime? shipDate = null)
    {
        // helpful to make sure we have business days and not regular days.
        var solidDate = shipDate ?? DateTime.Now.AddBusinessDays(1);

        serviceType ??= StampsServiceType.Unknown;

        // configure possible shipments
        if (serviceType == StampsServiceType.Unknown ||
            (!serviceType.Description.Contains("First Class") && !serviceType.Description.Contains("Priority")))
        {
            if (destination.IsUnitedStatesAddress())
            {
                CreateDomesticShipments(origin, destination, package, sender, receiver, serviceType, solidDate);
            }
            else
            {
                CreateInternationalShipments(origin, destination, package, sender, receiver, serviceType, solidDate);
            }
        }

        // first class is chosen, configure 1 shipment for first class.
        else if (serviceType.Description.Contains("First Class"))
        {
            if (destination.IsUnitedStatesAddress())
            {
                ConfigureDomesticFirstClass(origin, destination, package, sender, receiver, solidDate);
            }
            else
            {
                ConfigureIntlFirstClass(origin, destination, package, sender, receiver, solidDate);
            }
        }

        // if priority or priority express is chosen, configure 1 shipment for priority
        else if (serviceType.Description.Contains("Priority"))
        {
            if (destination.IsUnitedStatesAddress())
            {
                ConfigureDomesticPriority(origin, destination, package, sender, receiver, serviceType, solidDate);
            }
            else
            {
                ConfigureIntlPriority(origin, destination, package, sender, receiver, serviceType, solidDate);
            }
        }
    }

    public List<(Shipment shipment, RateRequestDetails rateOptions)> Shipments { get; } = new List<(Shipment shipment, RateRequestDetails rateOptions)>();

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
        return new Package(PackageType.LargeEnvelopeOrFlat.Dimensions, weight, insuredValue, isSignatureRequired);
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
        return new Package(PackageType.ThickEnvelope.Dimensions, weight, insuredValue, isSignatureRequired);
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
        return new Package(PackageType.Package.Dimensions, weight, insuredValue, isSignatureRequired);
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
        return new Package(PackageType.SmallFlatRateBox.Dimensions, weight, insuredValue, isSignatureRequired);
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
        return new Package(PackageType.FlatRateBox.Dimensions, weight, insuredValue, isSignatureRequired);
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
        return new Package(PackageType.LargeFlatRateBox.Dimensions, weight, insuredValue, isSignatureRequired);
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
        return new Package(PackageType.RegionalRateBoxA.Dimensions, weight, insuredValue, isSignatureRequired);
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
        return new Package(PackageType.RegionalRateBoxB.Dimensions, weight, insuredValue, isSignatureRequired);
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
        return new Package(PackageType.RegionalRateBoxC.Dimensions, weight, insuredValue, isSignatureRequired);
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
        return new Package(PackageType.FlatRateEnvelope.Dimensions, weight, insuredValue, isSignatureRequired);
    }

    private void CreateInternationalShipments(
        Address origin,
        Address destination,
        Package package,
        ContactInfo sender,
        ContactInfo receiver,
        StampsServiceType serviceType,
        DateTime shipDate)
    {
        if (package.DimensionsExceedFirstClassInternationalService())
        {
            ConfigureIntlPriority(origin, destination, package, sender, receiver, serviceType, shipDate);
        }
        else
        {
            ConfigureIntlFirstClass(origin, destination, package, sender, receiver, shipDate);
            ConfigureIntlPriority(origin, destination, package, sender, receiver, serviceType, shipDate);
        }
    }

    private void CreateDomesticShipments(
        Address origin,
        Address destination,
        Package package,
        ContactInfo sender,
        ContactInfo receiver,
        StampsServiceType serviceType,
        DateTime shipDate)
    {
        // if package weight is > 1 lb only configure priority
        if (package.Weight > 0.999375m)
        {
            ConfigureDomesticPriority(origin, destination, package, sender, receiver, serviceType, shipDate);
        }
        else
        {
            ConfigureDomesticFirstClass(origin, destination, package, sender, receiver, shipDate);
            ConfigureDomesticPriority(origin, destination, package, sender, receiver, serviceType, shipDate);
        }
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
            DeclaredValue = package.InsuredValue,
            RegisteredValue = package.InsuredValue
        };

        var packageType = package.IsFlatRateEnvelope() ? PackageType.ThickEnvelope : PackageType.Package;

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
        StampsServiceType serviceType,
        DateTime shipDate)
    {
        if (package.Weight > 70)
        {
            throw new ArgumentException("Priority Mail package can't be greater than 70 lbs", nameof(package.Weight));
        }

        var packages = new List<Package> { package };

        var rateOptions = new RateRequestDetails()
        {
            DeclaredValue = package.InsuredValue,
            RegisteredValue = package.InsuredValue
        };

        var packageType = GetFlatRatePackage(package, serviceType);

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
            throw new ArgumentException("First Class Package International Service can't be greater than 4.4 lbs and and packages cannot be more than 24 inches long and 36 inches in combined dimensions", nameof(package.Weight));
        }

        var packages = new List<Package> { package };

        // will need to add commodity & customs information here.
        var rateOptions = new RateRequestDetails()
        {
            DeclaredValue = package.InsuredValue,
            RegisteredValue = package.InsuredValue
        };

        var packageType = package.IsInternationalLargeEnvelope() ? PackageType.LargeEnvelopeOrFlat : PackageType.Package;
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
        StampsServiceType serviceType,
        DateTime shipDate)
    {
        if (package.Weight > 70)
        {
            throw new ArgumentException("Priority Mail package can't be greater than 70 lbs", nameof(package.Weight));
        }

        var packages = new List<Package> { package };

        var packageType = GetInternationalFlatRatePackage(package, serviceType);

        var shipOptions = new ShipmentOptions(packageType.Name, shipDate);

        // will need to add commodity & customs information here.
        var rateOptions = new RateRequestDetails()
        {
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

    private PackageType GetFlatRatePackage(Package package, StampsServiceType serviceType)
    {
        // sequence from smallest to largest

        if (package.IsFlatRateEnvelope())
        {
            return PackageType.FlatRatePaddedEnvelope;
        }

        if (package.IsPaddedFlatRateEnvelope())
        {
            return PackageType.FlatRatePaddedEnvelope;
        }

        if (package.IsLegalFlatRateEnvelope())
        {
            return PackageType.FlatRatePaddedEnvelope;
        }

        if (serviceType == StampsServiceType.PriorityExpress)
        {
            return PackageType.Package;
        }

        if (package.IsSmallFlatRateBox())
        {
            return PackageType.SmallFlatRateBox;
        }

        if (package.IsMediumFlatRateBox())
        {
            return PackageType.FlatRateBox;
        }

        if (package.IsLargeFlatRateBox())
        {
            return PackageType.LargeFlatRateBox;
        }

        return PackageType.Package;
    }

    private PackageType GetInternationalFlatRatePackage(Package package, StampsServiceType serviceType)
    {
        if (package.IsInternationalFlatRateEnvelope())
        {
            return PackageType.FlatRateEnvelope;
        }

        if (package.IsInternationalPaddedFlatRateEnvelope())
        {
            return PackageType.FlatRatePaddedEnvelope;
        }

        if (package.IsInternationalLegalFlatRateEnvelope())
        {
            return PackageType.LegalFlatRateEnvelope;
        }

        if (serviceType == StampsServiceType.PriorityExpressInternational)
        {
            return PackageType.Package;
        }

        if (package.IsInternationalSmallFlatRateBox())
        {
            return PackageType.SmallFlatRateBox;
        }

        if (package.IsInternationalMediumFlatRateBox())
        {
            return PackageType.FlatRateBox;
        }

        if (package.IsInternationalLargeFlatRateBox())
        {
            return PackageType.LargeFlatRateBox;
        }

        return PackageType.Package;
    }
}
