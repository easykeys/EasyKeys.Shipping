
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

    public async Task<Shipment> GetRatesManyAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        try
        {
            var body = new SupermodelIoLogisticsExpressRateRequest
            {
                CustomerDetails = new CustomerDetails4
                {
                    ShipperDetails = new SupermodelIoLogisticsExpressAddressRatesRequest
                    {
                        PostalCode = shipment.OriginAddress.PostalCode,
                        CityName = shipment.OriginAddress.City,
                        CountryCode = shipment.OriginAddress.CountryCode,
                        AddressLine1 = shipment.OriginAddress.StreetLine,
                        AddressLine2 = string.IsNullOrEmpty(shipment.OriginAddress.StreetLine2) ? null : shipment.OriginAddress.StreetLine2,
                        AddressLine3 = shipment.OriginAddress.StateOrProvince
                    },
                    ReceiverDetails = new SupermodelIoLogisticsExpressAddressRatesRequest
                    {
                        PostalCode = shipment.DestinationAddress.PostalCode,
                        CityName = shipment.DestinationAddress.City,
                        CountryCode = shipment.DestinationAddress.CountryCode,
                        AddressLine1 = string.IsNullOrEmpty(shipment.DestinationAddress.StreetLine) ? null : shipment.DestinationAddress.StreetLine,
                        AddressLine2 = string.IsNullOrEmpty(shipment.DestinationAddress.StreetLine2) ? null : shipment.DestinationAddress.StreetLine2,
                        AddressLine3 = string.IsNullOrEmpty(shipment.DestinationAddress.StateOrProvince) ? null : shipment.DestinationAddress.StateOrProvince
                    }
                },
                Accounts = new[]
                {
                    new SupermodelIoLogisticsExpressAccount
                    {
                        Number = shipment.Options.CustomerDHLExpressAccountNumber ?? _options.AccountNumber,
                        TypeCode = SupermodelIoLogisticsExpressAccountTypeCode.Shipper
                    }
                },
                PlannedShippingDateAndTime = shipment.Options.ShippingDate.ToString("yyyy-MM-dd'T'HH:mm:ss") + " GMT+00:00",
                UnitOfMeasurement = SupermodelIoLogisticsExpressRateRequestUnitOfMeasurement.Imperial,
                IsCustomsDeclarable = true,
                EstimatedDeliveryDate = new EstimatedDeliveryDate3
                {
                    IsRequested = true,
                    TypeCode = EstimatedDeliveryDate3TypeCode.QDDC
                },
                ValueAddedServices = AddServices(shipment),
                Packages = shipment.Packages.Select(x =>
                {
                    return new SupermodelIoLogisticsExpressPackageRR
                    {
                        Weight = (double)x.RoundedWeight,
                        Dimensions = new Dimensions3
                        {
                            Length = (double)x.Dimensions.Length,
                            Width = (double)x.Dimensions.Width,
                            Height = (double)x.Dimensions.Height
                        }
                    };
                }).ToArray(),
                ProductTypeCode = SupermodelIoLogisticsExpressRateRequestProductTypeCode.All,
                PayerCountryCode = shipment.OriginAddress.CountryCode,
                NextBusinessDay = true
            };

            var result = await _dhlExpressApi.ExpApiRatesManyAsync(body, cancellationToken: cancellationToken);

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

    private SupermodelIoLogisticsExpressValueAddedServicesRates[]? AddServices(Shipping.Abstractions.Models.Shipment shipment)
    {
        var valueAddedServices = new List<SupermodelIoLogisticsExpressValueAddedServicesRates>();

        if (shipment.Packages.Any(x => x.SignatureRequiredOnDelivery))
        {
            _logger.LogInformation("{providerName} : Adding Signature value", nameof(DHLExpressRateProvider));
            valueAddedServices.Add(new SupermodelIoLogisticsExpressValueAddedServicesRates
            {
                ServiceCode = "SF",
                Value = null,
                Currency = null
            });
        }

        if (shipment.Packages.Any(x => x.InsuredValue > 0m))
        {
            _logger.LogInformation("{providerName} : Adding Insurance value of {value}", nameof(DHLExpressRateProvider), shipment.Packages.First(x => x.InsuredValue > 0m).InsuredValue);

            valueAddedServices.Add(new SupermodelIoLogisticsExpressValueAddedServicesRates
            {
                ServiceCode = "II",
                Value = (double)shipment.Packages.Max(x => x.InsuredValue),
                Currency = "USD"
            });
        }

        return valueAddedServices.Any() ? valueAddedServices.ToArray() : null;
    }
}
