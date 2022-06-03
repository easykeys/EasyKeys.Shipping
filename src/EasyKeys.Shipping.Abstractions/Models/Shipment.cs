using System.Collections.ObjectModel;

namespace EasyKeys.Shipping.Abstractions.Models;

public class Shipment
{
    public Shipment(
        Address originAddress,
        Address destinationAddress,
        IList<Package> packages,
        ShipmentOptions options)
    {
        OriginAddress = originAddress ?? throw new ArgumentNullException(nameof(originAddress));
        DestinationAddress = destinationAddress ?? throw new ArgumentNullException(nameof(destinationAddress));
        Packages = packages?.ToList()?.AsReadOnly() ?? throw new ArgumentNullException(nameof(packages));
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// A collection of the packages to be shipped.
    /// </summary>
    public ReadOnlyCollection<Package> Packages { get; }

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
    ///     Errors returned by service provider (e.g. 'Wrong postal code').
    /// </summary>
    public IList<Error> Errors { get; } = new List<Error>();

    /// <summary>
    ///     Internal library errors during interaction with service provider
    ///     (e.g. SoapException was thrown).
    /// </summary>
    public IList<string> InternalErrors { get; } = new List<string>();
}
