using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EasyKeys.Shipping.Abstractions.Models
{
    public class Shipment
    {
        public Shipment(Address originAddress, Address destinationAddress, List<Package> packages, ShipmentOptions? options = null)
        {
            OriginAddress = originAddress ?? throw new ArgumentNullException(nameof(originAddress));
            DestinationAddress = destinationAddress ?? throw new ArgumentNullException(nameof(destinationAddress));
            Packages = packages?.AsReadOnly() ?? throw new ArgumentNullException(nameof(packages));
            Options = options ?? new ShipmentOptions();
        }

        public ReadOnlyCollection<Package> Packages { get; }

        /// <summary>
        /// Destination Address of the shipment.
        /// </summary>
        public Address DestinationAddress { get; }

        /// <summary>
        /// Origin of the shipment.
        /// </summary>
        public Address OriginAddress { get; }

        public ShipmentOptions Options { get; }

        /// <summary>
        ///     Shipment rates.
        /// </summary>
        public virtual List<Rate> Rates { get; } = new List<Rate>();

        /// <summary>
        ///     Errors returned by service provider (e.g. 'Wrong postal code').
        /// </summary>
        public List<Error> Errors { get; } = new List<Error>();

        /// <summary>
        ///     Internal library errors during interaction with service provider
        ///     (e.g. SoapException was thrown).
        /// </summary>
        public List<string> InternalErrors { get; } = new List<string>();
    }
}
