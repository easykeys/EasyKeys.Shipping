using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services;

public interface IStampsClientService
{
    Task<CleanseAddressResponse> CleanseAddressAsync(CleanseAddressRequest request, CancellationToken cancellationToken);

    Task<GetRatesResponse> GetRatesAsync(GetRatesRequest request, CancellationToken cancellationToken);

    Task<CreateIndiciumResponse> CreateIndiciumAsync(CreateIndiciumRequest request, CancellationToken cancellationToken);

    Task<CancelIndiciumResponse> CancelIndiciumAsync(CancelIndiciumRequest request, CancellationToken cancellationToken);

    Task<TrackShipmentResponse> TrackShipmentAsync(TrackShipmentRequest request, CancellationToken cancellationToken);
}
