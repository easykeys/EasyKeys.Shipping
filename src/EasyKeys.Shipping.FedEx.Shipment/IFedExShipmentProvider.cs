using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Shipment.Models;

namespace EasyKeys.Shipping.FedEx.Shipment;

public interface IFedExShipmentProvider
{
    /// <summary>
    /// <para>Creates a fedex shipping label.</para>
    /// <para>
    /// A multiple - package shipment(MPS) consists of two or more packages shipped to the same recipient.
    /// The first package in the shipment request is considered the master package.
    /// To create a multiple - package shipment,
    /// • Include the shipment level information such as TotalWeight, PackageCount, SignatureOptions)
    /// on the master package. The SequenceID for this package is 1.
    /// • In the master package reply, assign the tracking number of the first package in the
    /// MasterTrackingID element for all subsequent packages.You must return the master tracking
    /// number and increment the package number(SequenceID) for subsequent packages.
    /// </para>
    /// </summary>
    /// <param name="serviceType">The type of the service to be used for the label generation.</param>
    /// <param name="shipment">The shipment.</param>
    /// <param name="shipmentDetails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ShipmentLabel> CreateShipmentAsync(
        FedExServiceType serviceType,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails shipmentDetails,
        CancellationToken cancellationToken = default);

    Task<ShipmentCancelledResult> CancelShipmentAsync(string trackingId, CancellationToken cancellationToken = default);
}
