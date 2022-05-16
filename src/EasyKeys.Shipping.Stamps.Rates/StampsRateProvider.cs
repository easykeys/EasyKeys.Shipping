using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;

using Microsoft.Extensions.Logging;

namespace EasyKeys.Shipping.Stamps.Rates;

public class StampsRateProvider : IStampsRateProvider
{
    private readonly IRatesService _ratesService;
    private readonly ILogger<StampsRateProvider> _logger;

    public StampsRateProvider(
        IRatesService ratesService,
        ILogger<StampsRateProvider> logger)
    {
        _ratesService = ratesService;
        _logger = logger;
    }

    public async Task<Shipment> GetRatesAsync(Shipment shipment, RateRequestDetails rateRequestDetails, CancellationToken cancellationToken = default)
    {
        try
        {
            var rates = await _ratesService.GetRatesResponseAsync(shipment, rateRequestDetails, cancellationToken);

            var packageType = PackageType.FromName(shipment.Options.PackagingType);

            foreach (var rate in rates)
            {
                shipment.Rates.Add(new Rate($"{rate.ServiceType}", $"{rate.ServiceDescription}", $"{packageType.Description} - {packageType.Dimensions.Length}x{packageType.Dimensions.Width}x{packageType.Dimensions.Height}", rate.Amount, rate.DeliveryDate));

                _logger.LogDebug($"{rate.ServiceType} : {rate.ServiceDescription}");

                _logger.LogDebug($" => Cost : {rate.Amount}");

                _logger.LogDebug($" => Delivery Days : {rate.DeliverDays}");
            }
        }
        catch (Exception ex)
        {
            shipment.InternalErrors.Add(ex.Message);
        }

        return shipment;
    }
}
