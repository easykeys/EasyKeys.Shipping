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

    public async Task<Shipment> GetRatesAsync(List<Shipment> shipments, RateRequestDetails rateRequestDetails, CancellationToken cancellationToken = default)
    {
        // loop through each possible shipment and concat the rates to the first shipment
        // then return the first shipment with all the rates
        try
        {
            foreach (var shipment in shipments)
            {
                var rates = await _ratesService.GetRatesResponseAsync(shipment, rateRequestDetails, cancellationToken);

                foreach (var rate in rates)
                {
                    shipments.FirstOrDefault().Rates.Add(new Rate($"{rate.ServiceType}", rate.ServiceDescription, rate.Amount, rate.DeliveryDate));

                    _logger.LogInformation($"{rate.ServiceType} : {rate.ServiceDescription}");

                    _logger.LogInformation($" => Cost : {rate.Amount}");

                    _logger.LogInformation($" => Delivery Days : {rate.DeliverDays}");
                }
            }

            var orderedRates = shipments.FirstOrDefault().Rates.OrderBy(x => x.Name)
                                    .OrderBy(x => x.TotalCharges)
                                    .ToArray();

            shipments.FirstOrDefault().Rates.Clear();

            // remove the most expensive duplicate rate in cases where there is more than one possible shipment
            for (var i = 0; i < orderedRates.Length; i++)
            {
                if (shipments.FirstOrDefault().Rates.Any(x => x.Name == orderedRates[i].Name))
                {
                    continue;
                }

                shipments.FirstOrDefault().Rates.Add(orderedRates[i]);
            }
        }
        catch (Exception ex)
        {
            shipments.FirstOrDefault().InternalErrors.Add(ex.Message);
        }

        return shipments.FirstOrDefault();
    }
}
