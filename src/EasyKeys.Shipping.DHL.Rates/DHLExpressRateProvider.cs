
using EasyKeys.Shipping.Abstractions.Extensions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.DHL.Abstractions.OpenApis.V2.Express;
using EasyKeys.Shipping.DHL.Abstractions.Options;

using Microsoft.Extensions.Logging;

namespace EasyKeys.Shipping.DHL.Rates;

public class DHLExpressRateProvider : IDHLExpressRateProvider
{
    private readonly DHLExpressApiOptions _options;
    private readonly ILogger<DHLExpressRateProvider> _logger;
    private readonly DHLExpressApi _dhlExpressApi;

    public DHLExpressRateProvider(
        DHLExpressApi dhlExpressApi,
        DHLExpressApiOptions options,
        ILogger<DHLExpressRateProvider> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dhlExpressApi = dhlExpressApi ?? throw new ArgumentNullException(nameof(dhlExpressApi));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<Shipment> GetRatesAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _dhlExpressApi.ExpApiRatesAsync(
                accountNumber: shipment.Options.CustomerDHLExpressAccountNumber ?? _options.AccountNumber,
                originCountryCode: shipment.OriginAddress.CountryCode,
                originPostalCode: shipment.OriginAddress.PostalCode,
                originCityName: shipment.OriginAddress.City,
                destinationCityName: shipment.DestinationAddress.City,
                destinationCountryCode: shipment.DestinationAddress.CountryCode,
                destinationPostalCode: shipment.DestinationAddress.PostalCode,
                weight: (double)Math.Round(shipment.GetTotalWeight(), 3),
                unitOfMeasurement: UnitOfMeasurement.Imperial,
                length: (double)(shipment.Packages.FirstOrDefault()?.Dimensions.Length ?? 0.0m),
                width: (double)(shipment.Packages.FirstOrDefault()?.Dimensions.Width ?? 0.0m),
                height: (double)(shipment.Packages.FirstOrDefault()?.Dimensions.Height ?? 0.0m),
                plannedShippingDate: shipment.Options.ShippingDate.ToString("yyyy-MM-dd"),
                isCustomsDeclarable: true,
                nextBusinessDay: true,
                strictValidation: false,
                getAllValueAddedServices: true,
                requestEstimatedDeliveryDate: true,
                estimatedDeliveryDateType: EstimatedDeliveryDateType.QDDC
            );

            foreach (var product in result.Products)
            {
                try
                {
                    shipment.Rates.Add(new Rate(
                        product.ProductCode,
                        product.ProductName,
                        "UNKWN",
                        (decimal)product.TotalPrice.First(x => x.CurrencyType == "BILLC").Price,
                        (decimal)product.TotalPrice.First(x => x.CurrencyType == "PULCL").Price,
                        DateTime.Parse(product.DeliveryCapabilities.EstimatedDeliveryDateAndTime)));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "{providerName} rate adding failed.", nameof(DHLExpressRateProvider));
                }
            }
        }
        catch (ApiException<SupermodelIoLogisticsExpressErrorResponse> ex)
        {
            var error = ex?.Result.Detail ?? string.Empty;
            if (ex?.Result?.AdditionalDetails?.Any() ?? false)
            {
                error += string.Join(",", ex.Result.AdditionalDetails);
            }

            _logger.LogError("{name} : {message}", nameof(DHLExpressRateProvider), error);
            shipment.InternalErrors.Add(error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{providerName} failed", nameof(DHLExpressRateProvider));
            shipment.InternalErrors.Add(ex?.Message ?? $"{nameof(DHLExpressRateProvider)} failed");
        }

        return shipment;
    }
}
