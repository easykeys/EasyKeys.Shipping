using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;

using Microsoft.Extensions.Logging;

namespace EasyKeys.Shipping.Stamps.Rates
{
    public class StampsRateProvider : IStampsRateProvider
    {
        private readonly IGetRatesV40 _ratesV40;
        private readonly ILogger<StampsRateProvider> _logger;

        public StampsRateProvider(IGetRatesV40 ratesV40, ILogger<StampsRateProvider> logger)
        {
            _ratesV40 = ratesV40;
            _logger = logger;
        }

        public async Task<Shipment> GetRatesAsync(
                                                       Shipment shipment,
                                                       ShipmentDetails details,
                                                       Abstractions.Models.ServiceType serviceType = Abstractions.Models.ServiceType.UNKNOWN,
                                                       CancellationToken cancellationToken = default)
        {
            var rates = await _ratesV40.GetRatesResponseAsync(shipment, details, serviceType, cancellationToken);

            foreach (var rate in rates)
            {
                var addons = rate.AddOns.Select(x => x.AddOnDescription).Flatten(",");

                var required = rate.RequiresAllOf?.Length;

                rate.InsuredValue = 100M;

                rate.AddOns = null;

                shipment.Rates.Add(new Shipping.Abstractions.Rate($"usps-{rate.ServiceType}", rate.ServiceDescription, rate.Amount, rate.DeliveryDate));

                _logger.LogInformation($"{rate.ServiceType} : {rate.ServiceDescription}");

                _logger.LogInformation($" => Cost : {rate.Amount}");

                _logger.LogInformation($" => Delivery Days : {rate.DeliverDays}");
            }

            return shipment;
        }
    }
}
