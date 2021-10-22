using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Rates;

public class FedExRateConfigurator
{
    public FedExRateConfigurator(
        Address origin,
        Address destination,
        Package package,
        DateTime? shipDate = null)
    {
        // no delay, FedEx orders are priority for business.
        var solidDate = shipDate ?? DateTime.Now;

        // add this options always
        ConfigureGroundPackage(origin, destination, package, solidDate);

        if (package.IsFedExEnvelope())
        {
            ConfigureFedExPackage(origin, destination, package, solidDate, FedExPackageType.FEDEX_ENVELOPE);
            return;
        }

        if (package.IsFedExPak())
        {
            ConfigureFedExPackage(origin, destination, package, solidDate, FedExPackageType.FEDEX_PAK);
            return;
        }

        ConfigureYourPackage(origin, destination, package, solidDate);
    }

    public List<(Shipment shipment, ServiceType serviceType)> Shipments { get; } = new List<(Shipment shipment, ServiceType serviceType)>();

    /// <summary>
    /// FEDEX_ENVELOPE 10 lbs l: 9 1/2 (9.50) x w:12 1/2 (12.50).
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    /// <returns></returns>
    public static Package GetFedExEnvelop(
        decimal weight,
        decimal insuredValue = 20,
        bool isSignatureRequired = false)
    {
        return new Package(9m, 12m, 0.25m, weight, insuredValue, signatureRequiredOnDelivery: isSignatureRequired);
    }

    /// <summary>
    /// FEDEX_PAK 50 lbs l: 12 1/4 (12.25)  w: 15 1/2 (15.50).
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    /// <returns></returns>
    public static Package GetFedExPak(
        decimal weight,
        decimal insuredValue = 20,
        bool isSignatureRequired = false)
    {
        return new Package(12m, 15m, 1.5m, weight, insuredValue, signatureRequiredOnDelivery: isSignatureRequired);
    }

    private void ConfigureGroundPackage(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate)
    {
        var packages = new List<Package> { package };

        var options = new ShipmentOptions
        {
            PackagingType = nameof(FedExPackageType.YOUR_PACKAGING),
            SaturdayDelivery = true,
            ShippingDate = shipDate,
            PreferredCurrencyCode = ShipmentOptions.DefaultCurrencyCode,
        };

        var shipment = new Shipment(origin, destination, packages, options);
        var serviceType = destination.IsResidential ? ServiceType.GROUND_HOME_DELIVERY : ServiceType.FEDEX_GROUND;

        Shipments.Add((shipment, serviceType));
    }

    private void ConfigureYourPackage(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate)
    {
        var packages = new List<Package> { package };

        var options = new ShipmentOptions
        {
            PackagingType = nameof(FedExPackageType.YOUR_PACKAGING),
            SaturdayDelivery = true,
            ShippingDate = shipDate,
            PreferredCurrencyCode = ShipmentOptions.DefaultCurrencyCode,
        };

        var shipment = new Shipment(origin, destination, packages, options);
        Shipments.Add((shipment, ServiceType.DEFAULT));
    }

    private void ConfigureFedExPackage(
        Address origin,
        Address destination,
        Package package,
        DateTime shipDate,
        FedExPackageType type)
    {
        var packages = new List<Package> { package };

        var options = new ShipmentOptions
        {
            PackagingType = type.ToString(),
            SaturdayDelivery = true,
            ShippingDate = shipDate,
            PreferredCurrencyCode = ShipmentOptions.DefaultCurrencyCode,
        };

        var shipment = new Shipment(origin, destination, packages, options);

        Shipments.Add((shipment, ServiceType.DEFAULT));
    }
}
