using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.RatesAndTransitTimes;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;
using EasyKeys.Shipping.FedEx.Rates.WebServices.Impl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyKeys.Shipping.FedEx.Rates.RestApi.Impl;
public class FedexRateProvider : IFedExRateProvider
{
    private readonly RatesAndTransientTimesApi _client;
    private readonly IFedexApiAuthenticatorService _authService;
    private readonly ILogger<FedexRateProvider> _logger;
    private readonly FedExApiOptions _options;

    public FedexRateProvider(
        IFedexApiAuthenticatorService authService,
        IOptionsMonitor<FedExApiOptions> optionsMonitor,
        RatesAndTransientTimesApi client,
        ILogger<FedexRateProvider> logger)
    {
        _authService = authService;
        _options = optionsMonitor.CurrentValue;
        _client = client;
        _logger = logger;
    }

    public async Task<Shipment> GetRatesAsync(Shipment shipment, FedExServiceType? serviceType = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var ratesRequest = new Full_Schema_Quote_Rate
            {
                AccountNumber = new AccountNumber
                {
                    Value = _options.FedExAccountNumber
                },
                CarrierCodes = new List<string> { "FDXE", "FDXG" },
                RateRequestControlParameters = new RateRequestControlParameters
                {
                    ReturnTransitTimes = true
                },
                RequestedShipment = new RequestedShipment
                {
                    Shipper = new RateParty
                    {
                        Address = new RateAddress
                        {
                            StateOrProvinceCode = shipment.OriginAddress.StateOrProvince,
                            City = shipment.OriginAddress.City,
                            PostalCode = shipment.OriginAddress.PostalCode,
                            CountryCode = shipment.OriginAddress.CountryCode,
                            Residential = shipment.OriginAddress.IsResidential
                        }
                    },
                    Recipient = new RateParty
                    {
                        Address = new RateAddress
                        {
                            StateOrProvinceCode = shipment.DestinationAddress.StateOrProvince,
                            City = shipment.DestinationAddress.City,
                            PostalCode = shipment.DestinationAddress.PostalCode,
                            CountryCode = shipment.DestinationAddress.CountryCode,
                            Residential = shipment.DestinationAddress.IsResidential
                        }
                    },
                    ShipDateStamp = shipment.Options.ShippingDate.ToString("yyyy-MM-dd"),
                    PackagingType = shipment.Options.PackagingType,
                    PickupType = RequestedShipmentPickupType.USE_SCHEDULED_PICKUP,
                    RateRequestType = [RateRequestType.ACCOUNT, RateRequestType.LIST],
                    TotalPackageCount = 1,
                    TotalWeight = (double)shipment.Packages.Sum(x => x.RoundedWeight),
                    RequestedPackageLineItems = shipment.Packages.Select(x => new RequestedPackageLineItem
                    {
                        Weight = new Weight_2
                        {
                            Units = "LB",
                            Value = (int)x.RoundedWeight
                        },
                        DeclaredValue = new Money
                        {
                            Currency = "USD",
                            Amount = (double)x.InsuredValue
                        },
                        PackageSpecialServices = x.SignatureRequiredOnDelivery ?
                        new PackageSpecialServicesRequested
                            {
                                SignatureOptionType = PackageSpecialServicesRequestedSignatureOptionType.INDIRECT
                            }
                        :
                        null
                    }).ToList()
                }
            };

            if (shipment.DestinationAddress.IsUnitedStatesAddress() is not true)
            {
                ratesRequest.RequestedShipment.CustomsClearanceDetail = new RequestedShipmentCustomsClearanceDetail
                {
                    Commodities = shipment.Packages.Select(x => new Abstractions.OpenApis.V1.RatesAndTransitTimes.Commodity
                    {
                        NumberOfPieces = 1,
                        Description = "keys and locks",
                        CountryOfManufacture = "US",
                        Weight = new Weight_2
                        {
                            Units = "LB",
                            Value = (int)x.RoundedWeight
                        },
                        Quantity = 1,
                        QuantityUnits = "EA",
                        UnitPrice = new UnitPrice
                        {
                            Currency = "USD",
                            Amount = (double)x.InsuredValue
                        },
                        CustomsValue = new Money
                        {
                            Currency = "USD",
                            Amount = (double)x.InsuredValue
                        }
                    }).ToList()
                };
            }

            var token = await _authService.GetTokenAsync(cancellationToken);

            var rates = await _client.Rate_and_Transit_timesAsync(
                ratesRequest,
                Guid.NewGuid().ToString(),
                "application/json",
                "en_US",
                token,
                cancellationToken);

            foreach (var rateDetail in rates.Output!.RateReplyDetails!)
            {
                try
                {
                    var listCharges = rateDetail!.RatedShipmentDetails!.FirstOrDefault(x => x.RateType == RatedShipmentDetailRateType.LIST)?.TotalNetCharge;
                    var rate = new Rate(
                            rateDetail!.ServiceType!,
                            rateDetail!.ServiceName!,
                            rateDetail!.PackagingType!,
                            (decimal)rateDetail!.RatedShipmentDetails!.First(x => x.RateType == RatedShipmentDetailRateType.ACCOUNT).TotalNetCharge,
                            (decimal)listCharges.GetValueOrDefault(),
                            DateTime.TryParse(rateDetail?.Commit?.DateDetail?.DayFormat, out var dateResult) ? dateResult : DateTime.MinValue);
                    shipment.Rates.Add(rate);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "{providerName} rate adding failed.", nameof(FedExRateProvider));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{providerName} failed", nameof(FedExRateProvider));
            shipment.InternalErrors.Add(ex?.Message ?? $"{nameof(FedExRateProvider)} failed");
        }

        return shipment;
    }
}
