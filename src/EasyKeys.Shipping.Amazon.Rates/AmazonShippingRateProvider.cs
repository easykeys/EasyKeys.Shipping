using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Amazon.Abstractions.OpenApis.V2.Shipping;
using EasyKeys.Shipping.Amazon.Abstractions.Services;

namespace EasyKeys.Shipping.Amazon.Rates;

public class AmazonShippingRateProvider : IAmazonShippingRateProvider
{
    private readonly IAmazonApiAuthenticatorService _authenticatorService;
    private readonly AmazonShippingApi _shippingApi;

    public AmazonShippingRateProvider(
        IAmazonApiAuthenticatorService authenticatorService,
        AmazonShippingApi shippingApi)
    {
        _authenticatorService = authenticatorService;
        _shippingApi = shippingApi;
    }

    public async Task<Shipment> GetRatesAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        try
        {
            var rateRequest = new GetRatesRequest()
            {
                ShipDate = shipment.Options.ShippingDate.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"),
                ShipTo = new Abstractions.OpenApis.V2.Shipping.Address()
                {
                    Name = "unknown name",
                    AddressLine1 = shipment.DestinationAddress.StreetLine,
                    AddressLine2 = shipment.DestinationAddress.StreetLine2,
                    StateOrRegion = shipment.DestinationAddress.StateOrProvince,
                    City = shipment.DestinationAddress.City,
                    CountryCode = shipment.DestinationAddress.CountryCode,
                    PostalCode = shipment.DestinationAddress.PostalCode,
                    Email = "unknown name",
                    PhoneNumber = "unknown phone number"
                },
                ShipFrom = new Abstractions.OpenApis.V2.Shipping.Address()
                {
                    AddressLine1 = shipment.OriginAddress.StreetLine,
                    AddressLine2 = shipment.OriginAddress.StreetLine2,
                    StateOrRegion = shipment.OriginAddress.StateOrProvince,
                    City = shipment.OriginAddress.City,
                    CountryCode = shipment.OriginAddress.CountryCode,
                    PostalCode = shipment.OriginAddress.PostalCode,
                    Email = "devs@easykeys.com",
                    CompanyName = "EasyKeys",
                    PhoneNumber = "unknown phone number"
                },
                Packages = new ()
                {
                    new ()
                    {
                        Dimensions = new ()
                        {
                            Unit = DimensionsUnit.INCH,
                            Length = 1,
                            Width = 1,
                            Height = 1
                        },
                        Weight = new ()
                        {
                            Unit = WeightUnit.POUND,
                            Value = (double)shipment.Packages.Sum(x => x.RoundedWeight)
                        },
                        InsuredValue = new ()
                        {
                            Value = (double)shipment.Packages.Sum(x => x.InsuredValue),
                            Unit = "USD"
                        },
                        PackageClientReferenceId = "packageClientReferenceId",
                        Items = new ()
                        {
                            new ()
                            {
                                Weight = new ()
                                {
                                    Unit = WeightUnit.POUND
                                },
                                LiquidVolume = new ()
                                {
                                    Unit = LiquidVolumeUnit.ML
                                },
                                Description = "asdf",
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
                shipment.InternalErrors.Add(error.Message);
            }
        }
        catch (ApiException ex)
        {
            shipment.InternalErrors.Add(ex.Message);
        }
        catch (Exception ex)
        {
            shipment.InternalErrors.Add(ex.Message);
        }

        return shipment;
    }
}
