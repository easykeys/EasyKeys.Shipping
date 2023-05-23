using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Rates;

/// <summary>
/// Generates a list of shipments and rate details.
/// These shipments are composed of the smallest and best priced packaging type for the service requested.
/// </summary>
public class StampsRateConfigurator
{
    private readonly PackageWeight _packageWeight;
    private readonly Address _origin;
    private readonly Address _destination;
    private readonly Package _package;
    private readonly DateTime _shipDate;

    public StampsRateConfigurator(
        Address origin,
        Address destination,
        Package package,
        DateTime? shipDate = null)
    {
        _packageWeight = new PackageWeight(package.Weight, isOunce: false);

        if (package.Dimensions.Girth >= 108)
        {
            throw new ArgumentException("USPS doesn't support this package size.", nameof(package.Dimensions));
        }

        _origin = origin;
        _destination = destination;
        _package = package;

        // helpful to make sure we have business days and not regular days.
        _shipDate = shipDate ?? DateTime.Now;

        // configure possible shipments
        if (destination.IsUnitedStatesAddress())
        {
            CreateDomesticShipments();
        }
        else
        {
            CreateInternationalShipments();
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
        return new Package(StampsPackageType.LargeEnvelopeOrFlat.MaxSize, weight, insuredValue, isSignatureRequired);
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
        return new Package(StampsPackageType.ThickEnvelope.MaxSize, weight, insuredValue, isSignatureRequired);
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
        return new Package(StampsPackageType.FlatRateEnvelope.MaxSize, weight, insuredValue, isSignatureRequired);
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
        return new Package(StampsPackageType.Package.MaxSize, weight, insuredValue, isSignatureRequired);
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
        return new Package(StampsPackageType.SmallFlatRateBox.MaxSize, weight, insuredValue, isSignatureRequired);
    }

    /// <summary>
    /// USPS medium flat rate box. A special 11” x 8 ½” x 5 ½” or 14” x 3.5” x 12” USPS box that clearly indicates “Medium Flat Rate Box”.
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    /// <returns></returns>
    public static Package GetMediumFlatBox(
        decimal weight,
        decimal insuredValue = 20,
        bool isSignatureRequired = false)
    {
        return new Package(StampsPackageType.MediumFlatRateBox.MaxSize, weight, insuredValue, isSignatureRequired);
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
        return new Package(StampsPackageType.LargeFlatRateBox.MaxSize, weight, insuredValue, isSignatureRequired);
    }

    /// <summary>
    /// Sets shipment based on the <see cref="StampsPackageType"/>.
    /// </summary>
    /// <param name="packageType"></param>
    /// <param name="useMax"></param>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    public void SetShipment(
            StampsPackageType packageType,
            bool useMax = false,
            decimal? weight = null,
            decimal? insuredValue = null,
            bool? isSignatureRequired = null)
    {
        var package = new Package(
            useMax ? packageType.MaxSize : packageType.MinSize,
            weight ?? _packageWeight.InPounds,
            insuredValue ?? _package.InsuredValue,
            isSignatureRequired ?? _package.SignatureRequiredOnDelivery);

        var shipmentOptions = new ShipmentOptions(packageType.Name, _shipDate);

        var packages = new List<Package> { package };
        var shipment = new Shipment(_origin, _destination, packages, shipmentOptions);

        Shipments.Add(shipment);
    }

    /// <summary>
    ///  Allow to set shipment and package without enumerating values.
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="size"></param>
    /// <param name="weight"></param>
    /// <param name="insuredValue"></param>
    /// <param name="isSignatureRequired"></param>
    public void SetShipment(
            string packageName,
            Dimensions size,
            decimal? weight = null,
            decimal? insuredValue = null,
            bool? isSignatureRequired = null)
    {
        var package = new Package(
                size,
                weight ?? _packageWeight.InPounds,
                insuredValue ?? _package.InsuredValue,
                isSignatureRequired ?? _package.SignatureRequiredOnDelivery);

        var shipmentOptions = new ShipmentOptions(packageName, _shipDate);

        var packages = new List<Package> { package };
        var shipment = new Shipment(_origin, _destination, packages, shipmentOptions);

        Shipments.Add(shipment);
    }

    private void CreateDomesticShipments()
    {
        // get qualified options based on the package weight
        var packageTypes = StampsPackageType.List.Where(x => x.MaxWeight.InPounds >= _packageWeight.InPounds
                                                            && x.MaxSize.Measurement >= _package.Dimensions.Measurement);
        CreateShipments(packageTypes);
    }

    private void CreateInternationalShipments()
    {
        // get qualified options based on the package weight
        var packageTypes = StampsPackageType.List.Where(x => x.MaxInternationalWeight.InPounds >= _packageWeight.InPounds
                                                                && x.MaxSize.Measurement >= _package.Dimensions.Measurement);

        CreateShipments(packageTypes);
    }

    private void CreateShipments(IEnumerable<StampsPackageType>? packageTypes)
    {
        if (packageTypes is null)
        {
            return;
        }

        var packages = new List<Package> { _package };

        foreach (var packageType in packageTypes)
        {
            var shipmentOptions = new ShipmentOptions(packageType.Name, _shipDate);

            var shipment = new Shipment(_origin, _destination, packages, shipmentOptions);

            Shipments.Add(shipment);
        }
    }
}
