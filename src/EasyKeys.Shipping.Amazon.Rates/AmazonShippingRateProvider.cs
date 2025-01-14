using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Amazon.Abstractions.OpenApis.V2.Shipping;
using EasyKeys.Shipping.Amazon.Abstractions.Options;
using EasyKeys.Shipping.Amazon.Abstractions.Services;
using EasyKeys.Shipping.Amazon.Rates.Models;

using Microsoft.Extensions.Logging;

namespace EasyKeys.Shipping.Amazon.Rates;

public class AmazonShippingRateProvider : IAmazonShippingRateProvider
{
    private readonly IAmazonApiAuthenticatorService _authenticatorService;
    private readonly AmazonShippingApiOptions _options;
    private readonly AmazonShippingApi _shippingApi;
    private readonly ILogger<AmazonShippingRateProvider> _logger;

    public AmazonShippingRateProvider(
        ILogger<AmazonShippingRateProvider> logger,
        AmazonShippingApiOptions options,
        IAmazonApiAuthenticatorService authenticatorService,
        AmazonShippingApi shippingApi)
    {
        _logger = logger;
        _options = options;
        _authenticatorService = authenticatorService;
        _shippingApi = shippingApi;
        _shippingApi.BaseUrl = _options.IsDevelopment ? _shippingApi.BaseUrl : "https://sellingpartnerapi-na.amazon.com";
    }

    public async Task<Shipment> GetRatesAsync(Shipment shipment,RateContactInfo rateContactInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            var rateRequest = new GetRatesRequest()
            {
                ShipDate = shipment.Options.ShippingDate.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"),
                ShipTo = new Abstractions.OpenApis.V2.Shipping.Address()
                {
                    AddressLine1 = shipment.DestinationAddress.StreetLine,
                    AddressLine2 = shipment.DestinationAddress.StreetLine2,
                    StateOrRegion = shipment.DestinationAddress.StateOrProvince,
                    City = shipment.DestinationAddress.City,
                    CountryCode = shipment.DestinationAddress.CountryCode,
                    PostalCode = shipment.DestinationAddress.PostalCode,
                    Name = rateContactInfo.RecipientContact.FullName,
                    Email = rateContactInfo.RecipientContact.Email,
                    CompanyName = rateContactInfo.RecipientContact.Company,
                    PhoneNumber = rateContactInfo.RecipientContact.PhoneNumber
                },
                ShipFrom = new Abstractions.OpenApis.V2.Shipping.Address()
                {
                    AddressLine1 = shipment.OriginAddress.StreetLine,
                    AddressLine2 = shipment.OriginAddress.StreetLine2,
                    StateOrRegion = shipment.OriginAddress.StateOrProvince,
                    City = shipment.OriginAddress.City,
                    CountryCode = shipment.OriginAddress.CountryCode,
                    PostalCode = shipment.OriginAddress.PostalCode,
                    Name = rateContactInfo.SenderContact.FullName,
                    Email = rateContactInfo.SenderContact.Email,
                    CompanyName = rateContactInfo.SenderContact.Company,
                    PhoneNumber = rateContactInfo.SenderContact.PhoneNumber
                },
                Packages = new ()
                {
                    new ()
                    {
                        Dimensions = new ()
                        {
                            Unit = DimensionsUnit.INCH,
                            Length = (double)shipment.Packages.Max(x => x.Dimensions.RoundedLength),
                            Width = (double)shipment.Packages.Max(x => x.Dimensions.RoundedWidth),
                            Height = (double)shipment.Packages.Max(x => x.Dimensions.RoundedHeight)
                        },
                        Weight = new ()
                        {
                            Unit = WeightUnit.POUND,
                            Value = (double)shipment.Packages.Sum(x => x.Weight)
                        },
                        InsuredValue = new ()
                        {
                            Value = (double)shipment.Packages.Sum(x => x.InsuredValue),
                            Unit = "USD"
                        },
                        PackageClientReferenceId = Guid.NewGuid().ToString(),
                        Items = new ()
                        {
                            new ()
                            {
                                Weight = new ()
                                {
                                    Unit = WeightUnit.POUND,
                                    Value = (double)shipment.Packages.Sum(x => x.Weight)
                                },
                                LiquidVolume = new ()
                                {
                                    Unit = LiquidVolumeUnit.ML
                                },
                                Description = "Package",
                                Quantity = 1
                            }
                        }
                    }
                },
                ChannelDetails = new ()
                {
                    ChannelType = ChannelType.EXTERNAL
                }
            };

            var token = await _authenticatorService.GetTokenAsync(cancellationToken);

            var result = await _shippingApi.GetRatesAsync(token, XAmznShippingBusinessId.AmazonShipping_US, rateRequest);

            foreach (var amazonRate in result.Payload.Rates)
            {
                var rate = new Shipping.Abstractions.Models.Rate(
                        amazonRate.ServiceName,
                        amazonRate.ServiceName,
                        amazonRate.CarrierName,
                        (decimal)amazonRate.TotalCharge.Value,
                        (decimal)amazonRate.TotalCharge.Value,
                        amazonRate.Promise.DeliveryWindow.End.UtcDateTime);
                shipment.Rates.Add(rate);
            }
        }
        catch (ApiException<ErrorList> ex)
        {
            foreach (var error in ex.Result.Errors)
            {
                shipment.InternalErrors.Add($"{error.Message}-{error.Details}");
            }

            _logger.LogError(ex, $"Error getting rates from Amazon Shipping API: {string.Join(",",ex.Result.Errors)}");
        }
        catch (ApiException ex)
        {
            shipment.InternalErrors.Add(ex.Message);
            _logger.LogError(ex, $"Error getting rates from Amazon Shipping API: {ex.Message}");
        }
        catch (Exception ex)
        {
            shipment.InternalErrors.Add(ex.Message);
            _logger.LogError(ex, $"Error getting rates from Amazon Shipping API: {ex.Message}");
        }

        return shipment;
    }
}
