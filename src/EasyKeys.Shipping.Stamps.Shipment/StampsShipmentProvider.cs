using EasyKeys.Shipping.Abstractions.Extensions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.Rates.Extensions;
using EasyKeys.Shipping.Stamps.Rates.Models;
using EasyKeys.Shipping.Stamps.Shipment.Extensions;
using EasyKeys.Shipping.Stamps.Shipment.Models;

using Microsoft.Extensions.Logging;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment;

public class StampsShipmentProvider : IStampsShipmentProvider
{
    private readonly IStampsClientService _stampsClient;
    private readonly ILogger<StampsShipmentProvider> _logger;

    public StampsShipmentProvider(
        IStampsClientService stampsClientService,
        ILogger<StampsShipmentProvider> logger)
    {
        _stampsClient = stampsClientService ?? throw new ArgumentNullException(nameof(stampsClientService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ShipmentLabel> CreateDomesticShipmentAsync(
        Shipping.Abstractions.Models.Shipment shipment,
        RateOptions rateOptions,
        ShipmentDetails shipmentDetails,
        CancellationToken cancellationToken)
    {
        var request = shipmentDetails.Map()
                                     .SetShipmentNotification(shipmentDetails, rateOptions.Recipient);

        request.Rate = shipment.MapToDomesticRate(rateOptions);
        request.RedirectTo = request.Rate.From;

        return await GetLabelAsync(request, shipmentDetails.LabelOptions.ImageType, cancellationToken);
    }

    public async Task<ShipmentLabel> CreateInternationalShipmentAsync(
        Shipping.Abstractions.Models.Shipment shipment,
        RateInternationalOptions rateOptions,
        ShipmentDetails shipmentDetails,
        IList<Commodity> commodities,
        CustomsInformation customsInformation,
        CancellationToken cancellationToken)
    {
        var request = shipmentDetails.Map()
                      .SetShipmentNotification(shipmentDetails, rateOptions.Recipient)
                      .SetCustomsInformation(rateOptions.ContentType, commodities, customsInformation, (double)shipment.GetTotalWeight());

        request.Rate = shipment.MapToInternationalRate(rateOptions);
        request.RedirectTo = request.Rate.From;

        return await GetLabelAsync(request, shipmentDetails.LabelOptions.ImageType, cancellationToken);
    }

    public async Task<CancelIndiciumResponse> CancelShipmentAsync(string trackingId, CancellationToken cancellationToken)
    {
        var request = new CancelIndiciumRequest()
        {
            Item1 = trackingId
        };

        return await _stampsClient.CancelIndiciumAsync(request, cancellationToken);
    }

    private async Task<ShipmentLabel> GetLabelAsync(CreateIndiciumRequest request, Models.ImageType imageType, CancellationToken cancellationToken)
    {
        var shipmentLabel = new ShipmentLabel();

        try
        {
            var response = await _stampsClient.CreateIndiciumAsync(request, cancellationToken);

            shipmentLabel.Labels = new List<PackageLabelDetails>()
            {
                new PackageLabelDetails()
                {
                    ProviderLabelId = response.StampsTxID.ToString(),
                    ImageType = imageType.Name,
                    TrackingId = response.TrackingNumber,
                    Bytes = response.ImageData.ToList(),
                    TotalCharges = new ShipmentCharges()
                    {
                        SurchargesList = SetSurcharges(response),
                        Surcharges = SetSurcharges(response).Values.Sum(),
                        NetCharge = response.Rate.Amount
                    }
                }
            };
        }
        catch (Exception ex)
        {
            var error = ex?.InnerException?.Message ?? ex?.Message ?? string.Empty;
            _logger.LogError("{name} : {message}", nameof(StampsShipmentProvider), error);
            shipmentLabel.InternalErrors.Add(error);
        }

        return shipmentLabel;
    }

    private Dictionary<string, decimal> SetSurcharges(CreateIndiciumResponse response)
    {
        var surcharges = new Dictionary<string, decimal>();

        if (response?.Rate?.Surcharges == null)
        {
            return surcharges;
        }

        foreach (var surcharge in response.Rate.Surcharges)
        {
            surcharges[surcharge.SurchargeType.ToString()] = surcharge.Amount;
        }

        return surcharges;
    }
}
