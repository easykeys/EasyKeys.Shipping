using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Rates.WebServices.Extensions;

namespace EasyKeys.Shipping.FedEx.Rates;

public class FedExRateConfigurator
{
    public FedExRateConfigurator(
        Address origin,
        Address destination,
        Package package,
        bool isSaturdayDelivery = true,
        DateTime? shipDate = null)
    {
        // no delay, FedEx orders are priority for business.
        var solidDate = shipDate ?? DateTime.Now;

        // add this options always
        ConfigureGroundPackage(origin, destination, package, isSaturdayDelivery, solidDate);

        if (package.IsFedExEnvelope())
        {
            ConfigureFedExPackage(origin, destination, package, isSaturdayDelivery, solidDate, FedExPackageType.FedExEnvelope);
            return;
        }

        if (package.IsFedExPak())
        {
            ConfigureFedExPackage(origin, destination, package, isSaturdayDelivery, solidDate, FedExPackageType.FedExPak);
            return;
        }

        ConfigureYourPackage(origin, destination, package, isSaturdayDelivery, solidDate);
    }

    public List<(Shipment shipment, FedExServiceType serviceType)> Shipments { get; } = new List<(Shipment shipment, FedExServiceType serviceType)>();

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
        return new Package(
            FedExPackageType.FedExEnvelope.Dimensions,
            weight,
            insuredValue,
            signatureRequiredOnDelivery: isSignatureRequired);
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
        return new Package(
            FedExPackageType.FedExPak.Dimensions,
            weight,
            insuredValue,
            signatureRequiredOnDelivery: isSignatureRequired);
    }

    private void ConfigureGroundPackage(
        Address origin,
        Address destination,
        Package package,
        bool isSaturdayDelivery,
        DateTime shipDate)
    {
        // in order for the package to be send via ground, yourpacking must be selected.
        var packages = new List<Package> { package };

        var options = new ShipmentOptions(FedExPackageType.YourPackaging.Name, shipDate, isSaturdayDelivery);

        var shipment = new Shipment(origin, destination, packages, options);
        var serviceType = destination.IsResidential ? FedExServiceType.FedExGroundHomeDelivery : FedExServiceType.FedExGround;

        if (!destination.IsUnitedStatesAddress() && destination.IsCanadaAddress())
        {
            // fedex international ground is only for US/CA
            // https://www.fedex.com/en-ca/shipping-services/international/ground-shipping-to-the-us.html
            serviceType = FedExServiceType.FedExGround;
        }

        Shipments.Add((shipment, serviceType));
    }

    private void ConfigureYourPackage(
        Address origin,
        Address destination,
        Package package,
        bool isSaturdayDelivery,
        DateTime shipDate)
    {
        var packages = new List<Package> { package };

        var options = new ShipmentOptions(FedExPackageType.YourPackaging.Name, shipDate, isSaturdayDelivery);

        var shipment = new Shipment(origin, destination, packages, options);
        Shipments.Add((shipment, FedExServiceType.Default));
    }

    private void ConfigureFedExPackage(
        Address origin,
        Address destination,
        Package package,
        bool isSaturdayDelivery,
        DateTime shipDate,
        FedExPackageType type)
    {
        var packages = new List<Package> { package };

        var options = new ShipmentOptions(type.Name, shipDate, isSaturdayDelivery);

        var shipment = new Shipment(origin, destination, packages, options);

        Shipments.Add((shipment, FedExServiceType.Default));
    }
}
