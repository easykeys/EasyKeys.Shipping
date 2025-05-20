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
                    TotalWeight = (double)shipment.GetTotalWeight(),
                    RequestedPackageLineItems = shipment.Packages.Select(x => new RequestedPackageLineItem
                    {
                        CustomerReferences = [new() { CustomerReferenceType = CustomerReference_1CustomerReferenceType.INVOICE_NUMBER, Value = shipmentDetails.TransactionId }],
                        Weight = new Weight
                        {
                            Units = WeightUnits.LB,
                            Value = (double)x.Weight
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
                        },
                        PackageSpecialServices = new PackageSpecialServicesRequested()
                        {
                            SignatureOptionType = shipmentDetails.DeliverySignatureOptions.ToLower() switch
                            {
                                "service_default" => PackageSpecialServicesRequestedSignatureOptionType.SERVICE_DEFAULT,
                                "adult" => PackageSpecialServicesRequestedSignatureOptionType.ADULT,
                                "direct" => PackageSpecialServicesRequestedSignatureOptionType.DIRECT,
                                "indirect" => PackageSpecialServicesRequestedSignatureOptionType.INDIRECT,
                                "nosignaturerequired" => PackageSpecialServicesRequestedSignatureOptionType.NO_SIGNATURE_REQUIRED,
                                _ => PackageSpecialServicesRequestedSignatureOptionType.SERVICE_DEFAULT
                            },
                            SpecialServiceTypes = ["SIGNATURE_OPTION"]
                        }
                    }).ToList(),
                    PickupType = RequestedShipmentPickupType.USE_SCHEDULED_PICKUP,
                    LabelSpecification = new LabelSpecification
                    {
                        LabelPrintingOrientation = LabelSpecificationLabelPrintingOrientation.BOTTOM_EDGE_OF_TEXT_FIRST,
                        LabelRotation = LabelSpecificationLabelRotation.NONE,
                        LabelFormatType = LabelSpecificationLabelFormatType.COMMON2D,
                        ImageType = shipmentDetails.LabelOptions.ImageType.ToUpper() switch
                        {
                            "PNG" => LabelSpecificationImageType.PNG,
                            "PDF" => LabelSpecificationImageType.PDF,
                            "ZPL" => LabelSpecificationImageType.ZPLII,
                            _ => LabelSpecificationImageType.PNG,
                        },

                        LabelStockType = shipmentDetails.LabelOptions.LabelSize.ToUpper() switch
                        {
                            "PAPER_LETTER" => LabelSpecificationLabelStockType.PAPER_LETTER,
                            "PAPER_4X6" => LabelSpecificationLabelStockType.PAPER_4X6,
                            "PAPER_4X8" => LabelSpecificationLabelStockType.PAPER_4X8,
                            "STOCK_4X6" => LabelSpecificationLabelStockType.STOCK_4X6,
                            _ => LabelSpecificationLabelStockType.PAPER_4X6
                        },
                    },
                    EmailNotificationDetail = new ShipShipmentEMailNotificationDetail
                    {
                        AggregationType = ShipShipmentEMailNotificationDetailAggregationType.PER_PACKAGE,
                        EmailNotificationRecipients =
                        [
                            new ShipShipmentEmailNotificationRecipient
                            {
                                NotificationEventType = shipmentDetails.EmailNotification.EmailNotificationTypes.Select(x => x.Name.ToEnum<NotificationEventType>()).ToList(),
                                Name = shipmentDetails.Recipient.FullName,
                                EmailNotificationRecipientType = shipmentDetails.EmailNotification.NotificationRoleType.ToLower() switch
                                {
                                    "broker" => ShipShipmentEmailNotificationRecipientEmailNotificationRecipientType.BROKER,
                                    "third_party" => ShipShipmentEmailNotificationRecipientEmailNotificationRecipientType.THIRD_PARTY,
                                    "shipper" => ShipShipmentEmailNotificationRecipientEmailNotificationRecipientType.SHIPPER,
                                    "recipient" => ShipShipmentEmailNotificationRecipientEmailNotificationRecipientType.RECIPIENT,
                                    "thirdparty" => ShipShipmentEmailNotificationRecipientEmailNotificationRecipientType.THIRD_PARTY,
                                    _ => ShipShipmentEmailNotificationRecipientEmailNotificationRecipientType.RECIPIENT
                                },
                                EmailAddress = shipmentDetails.Recipient.Email,
                                NotificationFormatType = ShipShipmentEmailNotificationRecipientNotificationFormatType.TEXT,
                                NotificationType = ShipShipmentEmailNotificationRecipientNotificationType.EMAIL,
                                Locale = "en_US"
                            }

                        ],
                        PersonalMessage = shipmentDetails.EmailNotification.PersonalMessage,
                    }
                }
            };

            if (shipment.DestinationAddress.IsUnitedStatesAddress() is not true || shipment.DestinationAddress.IsUnitedStatesTerritory())
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
                                Type = CustomerImageUsageType.SIGNATURE,
                                Id = CustomerImageUsageId.IMAGE_1
                            },
                            new CustomerImageUsage
                            {
                                Type = CustomerImageUsageType.LETTER_HEAD,
                                Id = CustomerImageUsageId.IMAGE_2
                            }

                        ]
                    },
                };

                shipmentRequest.RequestedShipment.CustomsClearanceDetail = new CustomsClearanceDetail
                {
                    ExportDetail = GetExportDetail(shipment, shipmentDetails),
                    CommercialInvoice = new CommercialInvoice
                    {
                        ShipmentPurpose = CommercialInvoiceShipmentPurpose.SOLD,
                        CustomerReferences =
                        [
                            new CustomerReference
                            {
                                CustomerReferenceType = shipmentDetails.CustomerReferenceType.ToLower() switch
                                {
                                    "customer_reference" => CustomerReferenceType.CUSTOMER_REFERENCE,
                                    "department_number" => CustomerReferenceType.DEPARTMENT_NUMBER,
                                    "intracountry_regulatory_reference" => CustomerReferenceType.INTRACOUNTRY_REGULATORY_REFERENCE,
                                    "invoice_number" => CustomerReferenceType.INVOICE_NUMBER,
                                    "po_number" => CustomerReferenceType.P_O_NUMBER,
                                    "rma_association" => CustomerReferenceType.RMA_ASSOCIATION,
                                    _ => CustomerReferenceType.CUSTOMER_REFERENCE
                                },
                                Value = shipmentDetails.TransactionId
                            }

                        ],
                        SpecialInstructions = (shipment.DestinationAddress.IsCanadaAddress() && shipmentDetails.Commodities.Sum(x => x.CustomsValue) <= 3300m) ||
                                                (shipment.DestinationAddress.IsMexicoAddress() && shipmentDetails.Commodities.Sum(x => x.CustomsValue) <= 1000m) ?
                                                "Simplified Low Value Certification/Statement (LVS): I hereby certify that the goods covered by this shipment qualify as an originating good for the purposes of preferential tariff treatment under USMCA/T-MEC/CUSMA"
                                                : null
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

                        ExportLicenseExpirationDate = string.IsNullOrEmpty(x.ExportLicenseNumber) ? null : x.ExportLicenseExpirationDate,

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

                // customs payment type
                switch (shipmentDetails.CustomsPaymentType.Name)
                {
                    case "SENDER":
                        shipmentRequest.RequestedShipment.CustomsClearanceDetail.DutiesPayment = new Payment_1
                        {
                            PaymentType = Payment_1PaymentType.SENDER
                        };
                        break;
                    case "RECIPIENT":
                        shipmentRequest.RequestedShipment.CustomsClearanceDetail.DutiesPayment = new Payment_1
                        {
                            PaymentType = Payment_1PaymentType.RECIPIENT,
                            Payor = string.IsNullOrEmpty(shipmentDetails.AccountNumber) ? null : new Payor_1
                            {
                                ResponsibleParty = new Party_2
                                {
                                    AccountNumber = new PartyAccountNumber
                                    {
                                        Value = shipmentDetails.AccountNumber
                                    }
                                }
                            }
                        };
                        break;

                    case "THIRD_PARTY":
                        shipmentRequest.RequestedShipment.CustomsClearanceDetail.DutiesPayment = new Payment_1
                        {
                            PaymentType = Payment_1PaymentType.THIRD_PARTY,
                            Payor = string.IsNullOrEmpty(shipmentDetails.AccountNumber) ? null : new Payor_1
                            {
                                ResponsibleParty = new Party_2
                                {
                                    AccountNumber = new PartyAccountNumber
                                    {
                                        Value = shipmentDetails.AccountNumber
                                    }
                                }
                            }
                        };
                        break;
                    default:
                        shipmentRequest.RequestedShipment.CustomsClearanceDetail.DutiesPayment = new Payment_1
                        {
                            PaymentType = Payment_1PaymentType.RECIPIENT
                        };
                        break;
                }
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

                case "THIRD_PARTY":
                    shipmentRequest.RequestedShipment.ShippingChargesPayment = new Payment
                    {
                        PaymentType = PaymentType.THIRD_PARTY,
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
                default:
                    shipmentRequest.RequestedShipment.ShippingChargesPayment = new Payment
                    {
                        PaymentType = PaymentType.SENDER
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
                var rateDetail = createdShipment?.CompletedShipmentDetail?.ShipmentRating?.ShipmentRateDetails?.FirstOrDefault();

                var baseCharge = (decimal?)rateDetail?.TotalBaseCharge ?? 0m;
                var netCharge = (decimal?)rateDetail?.TotalNetCharge ?? 0m;
                var surCharge = (decimal?)rateDetail?.TotalSurcharges ?? 0m;

                foreach (var piece in createdShipment?.PieceResponses!)
                {
                    if(createdShipment?.ShipmentDocuments != null)
                    {
                        foreach (var doc in createdShipment.ShipmentDocuments!)
                        {
                            label.ShippingDocuments.Add(new Document
                            {
                                DocumentName = doc.ContentType.ToString() !,
                                ImageType = doc.DocType!,
                                Bytes = [doc.EncodedLabel],
                                CopiesToPrint = doc.CopiesToPrint.ToString()
                            });
                        }
                    }

                    label.Labels.Add(new PackageLabelDetails
                    {
                        TotalCharges = new ShipmentCharges
                        {
                            NetCharge = netCharge,
                            Surcharges = surCharge,
                            BaseCharge = baseCharge
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

    private ExportDetail? GetExportDetail(Shipping.Abstractions.Models.Shipment shipment, ShipmentDetails shipmentDetails)
    {
        if (shipment.OriginAddress.IsUnitedStatesAddress() || shipment.OriginAddress.IsCanadaAddress())
        {
            return new ExportDetail
            {
                B13AFilingOption = (ExportDetailB13AFilingOption)shipmentDetails.ExportDetails.B13AFilingOption,
                ExportComplianceStatement = shipmentDetails.ExportDetails.ComplianceStatement
            };
        }

        return null;
    }
}
