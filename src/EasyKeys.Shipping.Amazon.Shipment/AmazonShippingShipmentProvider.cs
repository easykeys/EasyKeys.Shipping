using System.Collections.ObjectModel;
using System.Text;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Amazon.Abstractions.OpenApis.V2.Shipping;
using EasyKeys.Shipping.Amazon.Abstractions.Options;
using EasyKeys.Shipping.Amazon.Abstractions.Services;
using EasyKeys.Shipping.Amazon.Shipment.Models;
using Microsoft.Extensions.Logging;

namespace EasyKeys.Shipping.Amazon.Shipment;

public class AmazonShippingShipmentProvider : IAmazonShippingShipmentProvider
{
    private readonly AmazonShippingApi _shippingApi;
    private readonly AmazonShippingApiOptions _options;
    private readonly IAmazonApiAuthenticatorService _authenticatorService;
    private readonly ILogger<AmazonShippingShipmentProvider> _logger;

    public AmazonShippingShipmentProvider(
        ILogger<AmazonShippingShipmentProvider> logger,
        AmazonShippingApiOptions options,
        AmazonShippingApi amazonShippingApi,
        IAmazonApiAuthenticatorService amazonApiAuthenticator)
    {
        _logger = logger;
        _options = options;
        _shippingApi = amazonShippingApi;
        _authenticatorService = amazonApiAuthenticator;
        _shippingApi.BaseUrl = _options.IsDevelopment ? _shippingApi.BaseUrl : "https://sellingpartnerapi-na.amazon.com";
    }

    public Task<ShipmentLabel> CreateShipmentAsync(
        string RateId,
        Shipping.Abstractions.Models.Shipment shipment,
        ShippingDetails shippingDetails,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ShipmentLabel> CreateSmartShipmentAsync(
        Shipping.Abstractions.Models.Shipment shipment,
        ShippingDetails shippingDetails,
        CancellationToken cancellationToken = default)
    {
        var label = new ShipmentLabel();

        try
        {
            var request = new OneClickShipmentRequest()
            {
                ShipDate = shipment.Options.ShippingDate.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"),
                ShipTo = new Abstractions.OpenApis.V2.Shipping.Address()
                {
                    Name = shippingDetails.Recipient.FullName,
                    CompanyName = shippingDetails.Recipient.Company,
                    AddressLine1 = shipment.DestinationAddress.StreetLine,
                    AddressLine2 = shipment.DestinationAddress.StreetLine2,
                    StateOrRegion = shipment.DestinationAddress.StateOrProvince,
                    City = shipment.DestinationAddress.City,
                    CountryCode = shipment.DestinationAddress.CountryCode,
                    PostalCode = shipment.DestinationAddress.PostalCode,
                    Email = shippingDetails.Recipient.Email,
                    PhoneNumber = shippingDetails.Recipient.PhoneNumber
                },
                ShipFrom = new Abstractions.OpenApis.V2.Shipping.Address()
                {
                    AddressLine1 = shipment.OriginAddress.StreetLine,
                    AddressLine2 = shipment.OriginAddress.StreetLine2,
                    StateOrRegion = shipment.OriginAddress.StateOrProvince,
                    City = shipment.OriginAddress.City,
                    CountryCode = shipment.OriginAddress.CountryCode,
                    PostalCode = shipment.OriginAddress.PostalCode,
                    Email = shippingDetails.Sender.Email,
                    CompanyName = shippingDetails.Sender.Company,
                    PhoneNumber = shippingDetails.Sender.PhoneNumber,
                    Name = shippingDetails.Sender.FullName
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
                        PackageClientReferenceId = shippingDetails.ReferenceId,
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
                                Description = "Package",
                                Quantity = 1
                            }
                        }
                    }
                },
                ChannelDetails = new ()
                {
                    ChannelType = ChannelType.EXTERNAL
                },
                ServiceSelection = new ()
                {
                    ServiceId = new () { shippingDetails.ServiceId }
                },
                LabelSpecifications = new ()
                {
                    Format = shippingDetails.LabelFormat switch
                    {
                        "PNG" => DocumentFormat.PNG,
                        "PDF" => DocumentFormat.PDF,
                        _ => DocumentFormat.PNG
                    },
                    Size = new ()
                    {
                        Length = (double)shippingDetails.LabelDimensions.Length,
                        Width = (double)shippingDetails.LabelDimensions.Width,
                        Unit = shippingDetails.LabelUnit switch
                        {
                            "INCH" => DocumentSizeUnit.INCH,
                            "CM" => DocumentSizeUnit.CENTIMETER,
                            _ => DocumentSizeUnit.INCH
                        }
                    },
                    Dpi = shippingDetails.LabelDpi,
                    PageLayout = "DEFAULT",
                    NeedFileJoining = false,
                    RequestedDocumentTypes = new Collection<DocumentType> { DocumentType.LABEL }
                }
            };

            var accessToken = await _authenticatorService.GetTokenAsync(cancellationToken);

            var shipmentResult = await _shippingApi
                .OneClickShipmentAsync(
                    accessToken,
                    XAmznShippingBusinessId3.AmazonShipping_US,
                    request,
                    cancellationToken);

            foreach (var details in shipmentResult.Payload.PackageDocumentDetails)
            {
                label.Labels.Add(
                    new PackageLabelDetails
                    {
                        TotalCharges = new ShipmentCharges
                        {
                            NetCharge = (decimal)shipmentResult.Payload.TotalCharge.Value,
                            Surcharges = 0.0m
                        },
                        ProviderLabelId = shipmentResult.Payload.ShipmentId,
                        TrackingId = details.TrackingId,
                        ImageType = "PNG",
                        Bytes = details.PackageDocuments.Select(x => Convert.FromBase64String(x.Contents)).ToList()
                    });
            }
        }
        catch (ApiException<ErrorList> ex)
        {
            foreach (var error in ex.Result.Errors)
            {
                label.InternalErrors.Add($"{error.Message}-{error.Details}");
            }

            _logger.LogError(ex, $"Error creating shipment from Amazon Shipping API: {string.Join(",", ex.Result.Errors)}");
        }
        catch (ApiException ex)
        {
            label.InternalErrors.Add(ex.Message);
            _logger.LogError(ex, $"Error creating shipment from Amazon Shipping API: {ex.Message}");
        }
        catch (Exception ex)
        {
            label.InternalErrors.Add(ex.Message);
            _logger.LogError(ex, $"Error creating shipment from Amazon Shipping API: {ex.Message}");
        }

        return label;
    }

    public async Task<ShipmentCancelledResult> CancelShipmentAsync(string shipmentId, CancellationToken cancellationToken = default)
    {
        var result = new ShipmentCancelledResult();
        try
        {
            var accessToken = await _authenticatorService.GetTokenAsync(cancellationToken);

            await _shippingApi.CancelShipmentAsync(
                shipmentId,
                accessToken,
                XAmznShippingBusinessId6.AmazonShipping_US,
                cancellationToken);
        }
        catch (ApiException<ErrorList> ex)
        {
            foreach (var error in ex.Result.Errors)
            {
                result.Errors.Add(error.Message);
            }
        }
        catch (ApiException ex)
        {
            result.Errors.Add(ex.Message);
        }
        catch (Exception ex)
        {
            result.Errors.Add(ex.Message);
        }

        return result;
    }
}
