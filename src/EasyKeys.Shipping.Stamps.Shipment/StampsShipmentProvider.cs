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

    public async Task<ShipmentLabel> CreateShipmentAsync(
        Shipping.Abstractions.Models.Shipment shipment,
        RateOptions rateOptions,
        ShipmentDetails shipmentDetails,
        CancellationToken cancellationToken = default)
    {
        var request = new CreateIndiciumRequest().MapToShipmentRequest(
            isDomestic: shipment.IsDomestic(),
            weightLb: (double)shipment.GetTotalWeight(),
            shipmentDetails: shipmentDetails,
            rateOptions: rateOptions);

        request.Rate = new RateV40().MapToRate(shipment, rateOptions);
        request.RedirectTo = request.Rate.From;

        return await GetLabelAsync(request, shipmentDetails.LabelOptions.ImageType, cancellationToken);
    }

    public async Task<ShipmentCancelledResult> CancelShipmentAsync(string trackingId, CancellationToken cancellationToken = default)
    {
        var result = new ShipmentCancelledResult();
        try
        {
            var request = new CancelIndiciumRequest()
            {
                Item1 = trackingId
            };

            await _stampsClient.CancelIndiciumAsync(request, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            result.Errors.Add(ex.Message);
        }

        return result;
    }

    private async Task<ShipmentLabel> GetLabelAsync(CreateIndiciumRequest request, Models.ImageType imageType, CancellationToken cancellationToken)
    {
        var shipmentLabel = new ShipmentLabel();

        try
        {
            var response = await _stampsClient.CreateIndiciumAsync(request, cancellationToken);
            var surcharge = SetSurcharges(response);

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
                        SurchargesList = surcharge,
                        Surcharges = surcharge.Values.Sum(),
                        NetCharge = response.Rate.Amount,
                        BaseCharge = response.Rate.Amount,
                    },
                    TotalCharges2 = new ShipmentCharges()
                    {
                        SurchargesList = surcharge,
                        Surcharges = surcharge.Values.Sum(),
                        NetCharge = response.Rate.Amount,
                        BaseCharge = response.Rate.Amount,
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
