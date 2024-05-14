using System.Reflection.Emit;
using System.Text;
using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Extensions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.Ship;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;
using EasyKeys.Shipping.FedEx.Shipment.Models;
using Microsoft.Extensions.Logging;

using Commodity = EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.Ship.Commodity;
using Dimensions = EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.Ship.Dimensions;

namespace EasyKeys.Shipping.FedEx.Shipment.RestApi.Impl;
public class FedExShipmentProvider : IFedExShipmentProvider
{
    private readonly ShipApi _client;
    private readonly IFedexApiAuthenticatorService _authService;
    private readonly ILogger<FedExShipmentProvider> _logger;
    private readonly FedExApiOptions _options;

    public FedExShipmentProvider(
        ShipApi client,
        IFedexApiAuthenticatorService authService,
        ILogger<FedExShipmentProvider> logger,
        FedExApiOptions options)
    {
        _client = client;
        _authService = authService;
        _logger = logger;
        _options = options;
    }

    public async Task<ShipmentLabel> CreateShipmentAsync(FedExServiceType serviceType, Shipping.Abstractions.Models.Shipment shipment, ShipmentDetails shipmentDetails, CancellationToken cancellationToken = default)
    {
        var label = new ShipmentLabel();

        try
        {
            var shipmentRequest = new Full_Schema_Ship
            {
                AccountNumber = new ShipperAccountNumber { Value = _options.FedExAccountNumber },
                LabelResponseOptions = LABELRESPONSEOPTIONS.LABEL,
                RequestedShipment = new RequestedShipment
                {
                    Shipper = new ShipperParty
                    {
                        Contact = new PartyContact
                        {
                            PersonName = shipmentDetails.Sender.FullName,
                            PhoneNumber = shipmentDetails.Sender.PhoneNumber,
                            CompanyName = shipmentDetails.Sender.Company,
                            EmailAddress = shipmentDetails.Sender.Email
                        },
                        Address = new PartyAddress
                        {
                            StreetLines = shipment.OriginAddress.GetStreetLines(),
                            StateOrProvinceCode = shipment.OriginAddress.StateOrProvince,
                            City = shipment.OriginAddress.City,
                            PostalCode = shipment.OriginAddress.PostalCode,
                            CountryCode = shipment.OriginAddress.CountryCode,
                            Residential = shipment.OriginAddress.IsResidential
                        }
                    },
                    Recipients = new List<RecipientsParty>
                    {
                        new RecipientsParty
                        {
                            Contact = new PartyContact
                            {
                                PersonName = shipmentDetails.Recipient.FullName,
                                PhoneNumber = shipmentDetails.Recipient.PhoneNumber,
                                CompanyName = shipmentDetails.Recipient.Company,
                                EmailAddress = shipmentDetails.Recipient.Email
                            },
                            Address = new PartyAddress
                            {
                                StreetLines = shipment.DestinationAddress.GetStreetLines(),
                                StateOrProvinceCode = shipment.DestinationAddress.StateOrProvince,
                                City = shipment.DestinationAddress.City,
                                PostalCode = shipment.DestinationAddress.PostalCode,
                                CountryCode = shipment.DestinationAddress.CountryCode,
                                Residential = shipment.DestinationAddress.IsResidential
                            }
                        }
                    },
                    ShipDatestamp = shipment.Options.ShippingDate.ToString("yyyy-MM-dd"),
                    ServiceType = serviceType.Name,
                    PackagingType = shipment.Options.PackagingType,
                    TotalPackageCount = shipment.Packages.Count,
                    TotalWeight = (double)shipment.Packages.Sum(x => x.RoundedWeight),
                    RequestedPackageLineItems = shipment.Packages.Select(x => new RequestedPackageLineItem
                    {
                        CustomerReferences = [new() { CustomerReferenceType = CustomerReference_1CustomerReferenceType.INVOICE_NUMBER, Value = shipmentDetails.TransactionId }],
                        Weight = new Weight
                        {
                            Units = WeightUnits.LB,
                            Value = (int)x.RoundedWeight
                        },
                        DeclaredValue = new Money
                        {
                            Currency = "USD",
                            Amount = (double)x.InsuredValue
                        },
                        Dimensions = new Dimensions
                        {
                            Length = (int)x.Dimensions.RoundedLength,
                            Width = (int)x.Dimensions.RoundedWidth,
                            Height = (int)x.Dimensions.RoundedHeight,
                            Units = DimensionsUnits.IN
                        }
                    }).ToList(),
                    PickupType = RequestedShipmentPickupType.USE_SCHEDULED_PICKUP,
                    LabelSpecification = new LabelSpecification
                    {
                        LabelPrintingOrientation = LabelSpecificationLabelPrintingOrientation.BOTTOM_EDGE_OF_TEXT_FIRST,
                        LabelRotation = LabelSpecificationLabelRotation.NONE,
                        LabelFormatType = LabelSpecificationLabelFormatType.COMMON2D,
                        ImageType = LabelSpecificationImageType.PNG,
                        LabelStockType = LabelSpecificationLabelStockType.PAPER_4X6
                    },
                    EmailNotificationDetail = new ShipShipmentEMailNotificationDetail
                    {
                        AggregationType = ShipShipmentEMailNotificationDetailAggregationType.PER_PACKAGE,
                        EmailNotificationRecipients =
                        [
                            new ShipShipmentEmailNotificationRecipient
                            {
                                Name = shipmentDetails.Recipient.FullName,
                                EmailNotificationRecipientType = ShipShipmentEmailNotificationRecipientEmailNotificationRecipientType.RECIPIENT,
                                EmailAddress = shipmentDetails.Recipient.Email,
                                NotificationFormatType = ShipShipmentEmailNotificationRecipientNotificationFormatType.TEXT,
                                NotificationType = ShipShipmentEmailNotificationRecipientNotificationType.EMAIL,
                                Locale = "en_US"
                            }

                        ]
                    }
                }
            };

            if (shipment.DestinationAddress.IsUnitedStatesAddress() is not true)
            {
                shipmentRequest.RequestedShipment.ShipmentSpecialServices = new ShipmentSpecialServicesRequested
                {
                    SpecialServiceTypes = ["ELECTRONIC_TRADE_DOCUMENTS"],
                    EtdDetail = new ETDDetail
                    {
                        RequestedDocumentTypes = [RequestedDocumentTypes.COMMERCIAL_INVOICE]
                    }
                };

                shipmentRequest.RequestedShipment.ShippingDocumentSpecification = new ShippingDocumentSpecification
                {
                    ShippingDocumentTypes = [ShippingDocumentTypes.COMMERCIAL_INVOICE],
                    CommercialInvoiceDetail = new CommercialInvoiceDetail
                    {
                        DocumentFormat = new ShippingDocumentFormat
                        {
                            DocType = ShippingDocumentFormatDocType.PDF,
                            StockType = ShippingDocumentFormatStockType.PAPER_LETTER
                        },
                        CustomerImageUsages =
                        [
                            new CustomerImageUsage
                            {
                                Type = CustomerImageUsageType.LETTER_HEAD,
                                Id = CustomerImageUsageId.IMAGE_1
                            },
                            new CustomerImageUsage
                            {
                                Type = CustomerImageUsageType.SIGNATURE,
                                Id = CustomerImageUsageId.IMAGE_2
                            }

                        ]
                    }
                };

                shipmentRequest.RequestedShipment.CustomsClearanceDetail = new CustomsClearanceDetail
                {
                    DutiesPayment = new Payment_1
                    {
                        PaymentType = shipmentDetails.PaymentType.Name == "SENDER" ?
                                Payment_1PaymentType.SENDER : Payment_1PaymentType.RECIPIENT
                    },
                    Commodities = shipmentDetails.Commodities.Select(x => new Commodity
                    {
                        Description = !string.IsNullOrEmpty(x.Description) ? x.Description : string.Empty,

                        Name = !string.IsNullOrEmpty(x.Name) ? x.Name : string.Empty,

                        NumberOfPieces = x.NumberOfPieces,

                        CountryOfManufacture = !string.IsNullOrEmpty(x.CountryOfManufacturer) ? x.CountryOfManufacturer : string.Empty,

                        Weight = new Weight_4()
                        {
                            Units = Weight_4Units.LB,
                            Value = (double)x.Weight
                        },

                        Quantity = x.Quantity,

                        QuantityUnits = !string.IsNullOrEmpty(x.QuantityUnits) ? x.QuantityUnits : string.Empty,

                        UnitPrice = new Money()
                        {
                            Amount = (double)x.UnitPrice,
                            Currency = shipment.Options.GetCurrencyCode()
                        },

                        HarmonizedCode = !string.IsNullOrEmpty(x.HarmonizedCode) ? x.HarmonizedCode : string.Empty,

                        ExportLicenseNumber = !string.IsNullOrEmpty(x.ExportLicenseNumber) ? x.ExportLicenseNumber : string.Empty,

                        ExportLicenseExpirationDate = x.ExportLicenseExpirationDate,

                        PartNumber = !string.IsNullOrEmpty(x.PartNumber) ? x.PartNumber : string.Empty,

                        Purpose = x.Purpose.ToUpper() switch
                        {
                            "BUSINESS" => CommodityPurpose.BUSINESS,
                            "CONSUMER" => CommodityPurpose.CONSUMER,
                            _ => CommodityPurpose.BUSINESS
                        },

                        CustomsValue = new Customs_Money()
                        {
                            Amount = (double)x.CustomsValue,
                            Currency = shipment.Options.GetCurrencyCode()
                        },

                        CIMarksAndNumbers = x.CIMarksandNumbers
                    }).ToList()
                };
            }
           
            switch (shipmentDetails.PaymentType.Name)
            {
                case "SENDER":
                    shipmentRequest.RequestedShipment.ShippingChargesPayment = new Payment
                    {
                        PaymentType = PaymentType.SENDER
                    };
                    break;
                case "RECIPIENT":
                    shipmentRequest.RequestedShipment.ShippingChargesPayment = new Payment
                    {
                        PaymentType = PaymentType.RECIPIENT,
                        Payor = new Payor
                        {
                            ResponsibleParty = new ResponsiblePartyParty
                            {
                                AccountNumber = new PartyAccountNumber
                                {
                                    Value = shipmentDetails.AccountNumber
                                }
                            }
                        }
                    };
                    break;
            }

            var token = await _authService.GetTokenAsync(cancellationToken);

            var response = await _client.Create_ShipmentAsync(
                shipmentRequest,
                Guid.NewGuid().ToString(),
                "application/json",
                "en_US",
                token,
                cancellationToken);

            foreach (var createdShipment in response.Output!.TransactionShipments!)
            {
                var netCharge = (decimal)createdShipment.CompletedShipmentDetail!.ShipmentRating!.ShipmentRateDetails!.Max(x => x.TotalNetCharge);
                var surCharge = (decimal)createdShipment.CompletedShipmentDetail!.ShipmentRating!.ShipmentRateDetails!.Max(x => x.TotalSurcharges);
                foreach (var piece in createdShipment.PieceResponses!)
                {
                    label.Labels.Add(new PackageLabelDetails
                    {
                        TotalCharges = new ShipmentCharges
                        {
                            NetCharge = netCharge,
                            Surcharges = surCharge
                        },
                        TrackingId = piece.TrackingNumber!,
                        ImageType = piece.PackageDocuments!.First().DocType!,
                        Bytes = [piece.PackageDocuments!.First().EncodedLabel!]
                    });
                }
            }

            return label;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{providerName} failed", nameof(FedExShipmentProvider));
            label.InternalErrors.Add(ex?.Message ?? $"{nameof(FedExShipmentProvider)} failed");
        }

        return label;
    }

    public async Task<ShipmentCancelledResult> CancelShipmentAsync(string trackingId, CancellationToken cancellationToken = default)
    {
        var cancelledResult = new ShipmentCancelledResult();
        try
        {
            var request = new Full_Schema_Cancel_Shipment
            {
                AccountNumber = new ShipperAccountNumber { Value = _options.FedExAccountNumber },
                TrackingNumber = trackingId,
                DeletionControl = Full_Schema_Cancel_ShipmentDeletionControl.DELETE_ALL_PACKAGES
            };

            var token = await _authService.GetTokenAsync(cancellationToken);

            var result = await _client.Cancel_ShipmentAsync(
                request,
                Guid.NewGuid().ToString(),
                "application/json",
                "en_US",
                token,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{providerName} failed", nameof(FedExShipmentProvider));
            cancelledResult.Errors.Add(ex?.Message ?? $"{nameof(FedExShipmentProvider)} failed");
        }

        return cancelledResult;
    }
}
