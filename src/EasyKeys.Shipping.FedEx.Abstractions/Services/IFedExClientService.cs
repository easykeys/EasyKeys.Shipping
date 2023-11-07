using AddressValidationClient.v4;

using RateClient.v28;

using ShipClient.v25;

using TrackClient.v19;

using UploadDocumentService_v19;

namespace EasyKeys.Shipping.FedEx.Abstractions.Services;

public interface IFedExClientService
{
    UploadDocumentPortType CreateUploadDocumentClient();

    TrackPortType CreateTrackClient();

    AddressValidationPortType CreateAddressValidationClient();

    RatePortType CreateRateClient();

    ShipPortType CreateShipClient();
}
