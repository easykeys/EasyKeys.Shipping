using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Rates.Client.V1;
using EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Address = EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request.Address;

namespace EasyKeys.Shipping.FedEx.Rates.RestApi.Impl;
public class FedexRateProvider : IFedExRateProvider
{
    private readonly IFedexRatesAndTransitTimesClient _client;
    private readonly ILogger<FedexRateProvider> _logger;
    private readonly FedExApiOptions _options;

    public FedexRateProvider(
        IOptionsMonitor<FedExApiOptions> optionsMonitor,
        IFedexRatesAndTransitTimesClient client,
        ILogger<FedexRateProvider> logger)
    {
        _options = optionsMonitor.CurrentValue;
        _client = client;
        _logger = logger;
    }

    public async Task<Shipment> GetRatesAsync(Shipment shipment, FedExServiceType? serviceType = null, CancellationToken cancellationToken = default)
    {
        var ratesRequest = new RequestRoot
        {
            AccountNumber = new AccountNumber { Value = shipment.Options.CustomerFedexAccountNumber ?? _options.FedExAccountNumber },
            CarrierCodes = new List<string> { "FDXE", "FDXG" },
            RateRequestControlParameters = new RateRequestControlParameters
            {
                ReturnTransitTimes = true,
            },
            RequestedShipment = new RequestedShipment
            {
                Shipper = new Shipper
                {
                    Address = new Address
                    {
                        PostalCode = shipment.OriginAddress.PostalCode,
                        CountryCode = shipment.OriginAddress.CountryCode,
                        Residential = shipment.OriginAddress.IsResidential
                    }
                },
                Recipient = new Recipient
                {
                    Address = new Address
                    {
                        PostalCode = shipment.DestinationAddress.PostalCode,
                        CountryCode = shipment.DestinationAddress.CountryCode,
                        Residential = shipment.DestinationAddress.IsResidential
                    }
                },
                ShipDateStamp = shipment.Options.ShippingDate.ToString("yyyy-MM-dd"),
                PackagingType = shipment.Options.PackagingType,
                PickupType = "USE_SCHEDULED_PICKUP",
                RateRequestType = ["ACCOUNT", "LIST"],
                RequestedPackageLineItems = shipment.Packages.Select(x => new RequestedPackageLineItem
                {
                    Weight = new Weight
                    {
                        Units = "LB",
                        Value = (int)x.RoundedWeight
                    },
                    DeclaredValue = new DeclaredValue
                    {
                        Currency = "USD",
                        Amount = x.InsuredValue.ToString()
                    }
                }).ToList(),
            }
        };
        if (shipment.DestinationAddress.IsUnitedStatesAddress() is not true)
        {
            ratesRequest.RequestedShipment.CustomsClearanceDetail = new CustomsClearanceDetail
            {
                Commodities = shipment.Packages.Select(x => new Client.V1.Models.Request.Commodity
                {
                    NumberOfPieces = 1,
                    Description = "keys and locks",
                    CountryOfManufacture = "US",
                    Weight = new Weight
                    {
                        Units = "LB",
                        Value = (int)x.RoundedWeight
                    },
                    Quantity = 1,
                    QuantityUnits = "EA",
                    UnitPrice = new UnitPrice
                    {
                        Currency = "USD",
                        Amount = x.InsuredValue.ToString()
                    },
                    CustomsValue = new CustomsValue
                    {
                        Currency = "USD",
                        Amount = x.InsuredValue.ToString()
                    }
                }).ToList()
            };
        }

        if (shipment.Options.PackagingType != FedExPackageType.YourPackaging.Name && shipment.DestinationAddress.IsUnitedStatesAddress())
        {
            ratesRequest.RequestedShipment.ShipmentSpecialServices = new ShipmentSpecialServices
            {
                SpecialServiceTypes = new List<string> { "FEDEX_ONE_RATE" }
            };
        }

        var rates = await _client.GetRatesAsync(ratesRequest, cancellationToken);

        foreach (var error in rates.Errors)
        {
            shipment.Errors.Add(new Error
            {
                Description = error.Message
            });
        }

        if (shipment.Errors.Any() || rates.Output?.RateReplyDetails == null)
        {
            return shipment;
        }

        foreach (var rateDetail in rates.Output.RateReplyDetails)
        {
            var rate = new Rate(
                rateDetail.ServiceType,
                rateDetail.ServiceName,
                rateDetail.PackagingType,
                (decimal)rateDetail.RatedShipmentDetails.First(x => x.RateType == "ACCOUNT").TotalNetCharge,
                (decimal)rateDetail.RatedShipmentDetails.First(x => x.RateType == "LIST").TotalNetCharge,
                rateDetail.OperationalDetail.CommitDate);
            shipment.Rates.Add(rate);
        }

        return shipment;
    }
}
