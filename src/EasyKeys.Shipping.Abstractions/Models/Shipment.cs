using System.Collections.ObjectModel;

namespace EasyKeys.Shipping.Abstractions.Models;

public class Shipment
{
    public Shipment(Address originAddress, Address destinationAddress, List<Package> packages, ShipmentOptions? options = null)
    {
        OriginAddress = originAddress ?? throw new ArgumentNullException(nameof(originAddress));
        DestinationAddress = destinationAddress ?? throw new ArgumentNullException(nameof(destinationAddress));
        Packages = packages?.AsReadOnly() ?? throw new ArgumentNullException(nameof(packages));
        Options = options ?? new ShipmentOptions();
    }

    public RecipientInformation RecipientInformation { get; set; } = new RecipientInformation();

    public SenderInformation SenderInformation { get; set; } = new SenderInformation();

    /// <summary>
    /// A collection of the packages to be shipped.
    /// </summary>
    public ReadOnlyCollection<Package> Packages { get; }

    /// <summary>
    /// Required for International Shipments Only.
    /// A collection shipment contents that are considered to be dutiable.
    /// </summary>
    public IList<Commodity> Commodities { get; } = new List<Commodity>();

    /// <summary>
    /// Destination Address of the shipment.
    /// </summary>
    public Address DestinationAddress { get; }

    /// <summary>
    /// Origin of the shipment.
    /// </summary>
    public Address OriginAddress { get; }

    /// <summary>
    /// <see cref="Shipment"/> configurable options.
    /// </summary>
    public ShipmentOptions Options { get; }

    /// <summary>
    /// Shipment rates.
    /// </summary>
    public virtual IList<Rate> Rates { get; } = new List<Rate>();

    /// <summary>
    ///  Warnings returned by service provder (e.g. 'Street Address, City, State, and Zip are valid, but a suite or apt number may be missing.').
    /// </summary>
    public IList<Error> Warnings { get; set; } = new List<Error>();

    /// <summary>
    ///     Errors returned by service provider (e.g. 'Wrong postal code').
    /// </summary>
    public IList<Error> Errors { get; } = new List<Error>();

    /// <summary>
    ///     Internal library errors during interaction with service provider
    ///     (e.g. SoapException was thrown).
    /// </summary>
    public IList<string> InternalErrors { get; } = new List<string>();
}
