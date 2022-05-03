using AddressValidationClient.v4;

using RateClient.v28;

using ShipClient.v25;

using TrackClient.v19;

namespace EasyKeys.Shipping.FedEx.Abstractions.Services
{
    public interface IFedExClientService
    {
        TrackPortType CreateTrackClient();

        AddressValidationPortType CreateAddressValidationClient();

        RatePortType CreateRateClient();

        ShipPortType CreateShipClient();
    }
}
