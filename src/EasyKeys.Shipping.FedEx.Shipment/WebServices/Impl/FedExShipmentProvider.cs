using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Extensions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;
using EasyKeys.Shipping.FedEx.Shipment.Models;
using EasyKeys.Shipping.FedEx.Shipment.WebServices.Extensions;

using Humanizer;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ShipClient.v25;

namespace EasyKeys.Shipping.FedEx.Shipment.WebServices.Impl;

public class FedExShipmentProvider : IFedExShipmentProvider
{
    private readonly ILogger<FedExShipmentProvider> _logger;
    private readonly ShipPortType _shipmentClient;
    private FedExOptions _options;

    public FedExShipmentProvider(
        IOptionsMonitor<FedExOptions> optionsMonitor,
        IFedExClientService fedExClientService,
        ILogger<FedExShipmentProvider> logger)
    {
        _options = optionsMonitor.CurrentValue;

        optionsMonitor.OnChange(x => _options = x);

        _shipmentClient = fedExClientService.CreateShipClient();

        _logger = logger ?? throw new ArgumentException(nameof(logger));
    }

    public async Task<ShipmentCancelledResult> CancelShipmentAsync(string trackingId, CancellationToken cancellationToken = default)
    {
        var client = _shipmentClient;
        var result = new ShipmentCancelledResult();
        try
        {
            // Create the delete shipment request
            var request = CreateDeleteShipmentRequest(trackingId);
            var deleteShipmentRequest = new deleteShipmentRequest1(request);

            var response = await client.deleteShipmentAsync(deleteShipmentRequest);

            // Handle the response
            if (response.ShipmentReply.HighestSeverity == NotificationSeverityType.SUCCESS)
            {
                return result;
            }
            else
            {
                result.Errors.Add("Code: {0} , Message: {1}".FormatWith(response.ShipmentReply.HighestSeverity, response.ShipmentReply.Notifications.Select(x => x.Message).Flatten(",")));
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add(ex.Message);
        }

        return result;
    }

    public async Task<ShipmentLabel> CreateShipmentAsync(
        FedExServiceType serviceType,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails shipmentDetails,
        CancellationToken cancellationToken = default)
    {
        var client = _shipmentClient;

        var label = new ShipmentLabel();

        var masterTrackingId = default(TrackingId);

        try
        {
            // for multiple packages, each package must have its own request.
            for (var i = 0; i < shipment.Packages.Count; i++)
            {
                var request = CreateShipmentRequest(
                        serviceType,
                        shipment,
                        shipmentDetails,
                        i);

                var shipmentRequest = new processShipmentRequest1(request);

                shipmentRequest.ProcessShipmentRequest.RequestedShipment.MasterTrackingId = masterTrackingId ?? null;

                var response = await client.processShipmentAsync(shipmentRequest);

                var reply = response?.ProcessShipmentReply;

                if (reply?.HighestSeverity != NotificationSeverityType.ERROR
                         && reply?.HighestSeverity != NotificationSeverityType.FAILURE)
                {
                    var totalCharges = new ShipmentCharges();
                    var totalCharges2 = new ShipmentCharges();

                    var packageDetails = reply?.CompletedShipmentDetail?.CompletedPackageDetails?.ToList() ?? new List<CompletedPackageDetail>();
                    var airWaybillLabels = packageDetails?.FirstOrDefault()?.PackageDocuments?.ToList() ?? new List<ShippingDocument>();
                    var rateDetails = reply?.CompletedShipmentDetail?.ShipmentRating?.ShipmentRateDetails?.ToList() ?? new List<ShipmentRateDetail>();
                    var shipmentDocuments = reply?.CompletedShipmentDetail?.ShipmentDocuments?.ToList() ?? new List<ShippingDocument>();

                    // international labels only
                    if (reply?.CompletedShipmentDetail?.ShipmentRating != null)
                    {
                        var payerAccounIntl = rateDetails.Where(x => x?.RateType == ReturnedRateType.PAYOR_ACCOUNT_SHIPMENT
                                                                      || x?.RateType == ReturnedRateType.PAYOR_ACCOUNT_PACKAGE);

                        if (payerAccounIntl.Any())
                        {
                            payerAccounIntl.SelectMany(x => x.Surcharges)
                                        .ToList()
                                        .ForEach(x => totalCharges.SurchargesList[x.Description] = x.Amount.Amount);

                            totalCharges.BaseCharge = payerAccounIntl.Sum(x => x.TotalBaseCharge.Amount);
                            totalCharges.NetCharge = payerAccounIntl.Sum(x => x.TotalNetCharge.Amount);
                            totalCharges.Surcharges = payerAccounIntl.Sum(x => x.TotalSurcharges.Amount);
                        }

                        var payerListIntl = rateDetails.Where(x => x?.RateType == ReturnedRateType.PAYOR_LIST_SHIPMENT
                                                                      || x?.RateType == ReturnedRateType.PAYOR_LIST_PACKAGE);

                        if (payerListIntl.Any())
                        {
                            payerListIntl.SelectMany(x => x.Surcharges)
                                        .ToList()
                                        .ForEach(x => totalCharges2.SurchargesList[x.Description] = x.Amount.Amount);

                            totalCharges2.BaseCharge = payerListIntl.Sum(x => x.TotalBaseCharge.Amount);
                            totalCharges2.NetCharge = payerListIntl.Sum(x => x.TotalNetCharge.Amount);
                            totalCharges2.Surcharges = payerListIntl.Sum(x => x.TotalSurcharges.Amount);
                        }

                        foreach (var document in shipmentDocuments)
                        {
                            label.ShippingDocuments.Add(
                                new Document()
                                {
                                    ImageType = document.ImageType.ToString() ?? string.Empty,
                                    Bytes = document?.Parts?.Select(x => x)?.Select(x => x.Image)?.ToList(),
                                    DocumentName = document?.Type.ToString() ?? string.Empty,
                                    CopiesToPrint = document?.CopiesToPrint ?? string.Empty
                                });
                        }

                        foreach (var awbLabels in airWaybillLabels)
                        {
                            label.Labels.Add(
                                new PackageLabelDetails
                                {
                                    TrackingId = "FEDEX AWB COPY - PLEASE PLACE IN POUCH",
                                    ImageType = awbLabels.ImageType.ToString(),
                                    Bytes = awbLabels.Parts.Select(x => x.Image)?.ToList()
                                });
                        }
                    }

                    // add byte array for actual generated lablels.
                    label.Labels.Add(
                                new PackageLabelDetails
                                {
                                    TotalCharges = totalCharges,

                                    TotalCharges2 = totalCharges2,

                                    TrackingId = packageDetails?.SelectMany(x => x.TrackingIds)?.Select(x => x.TrackingNumber)?.Flatten(";") ?? string.Empty,

                                    // ImageType = shipmentDetails.LabelOptions.ImageType,
                                    ImageType = packageDetails?.Select(x => x.Label.ImageType)?.FirstOrDefault().ToString() ?? string.Empty,

                                    Bytes = packageDetails?.Select(x => x.Label.Parts)?.SelectMany(x => x)?.Select(x => x.Image)?.ToList(),
                                });

                    masterTrackingId = reply?.CompletedShipmentDetail.MasterTrackingId;
                }
                else
                {
                    var errors = reply.Notifications.Select(x => x.Message).Flatten(",");

                    _logger.LogError("{providerName} failed: {errors}", nameof(FedExShipmentProvider), errors);

                    label.InternalErrors.Add(errors);
                }
            }

            return label;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{providerName} failed", nameof(FedExShipmentProvider));

            var error = ex?.InnerException?.Message ?? ex?.Message;
            label.InternalErrors.Add($"{nameof(FedExShipmentProvider)} failed: {error}");
        }

        return label;
    }

    private ProcessShipmentRequest CreateShipmentRequest(
        FedExServiceType serviceType,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails details,
        int sequenceNumber)
    {
        var request = CreateProcessShipmentRequest(details);

        SetShipmentDetails(
            request,
            serviceType,
            shipment,
            details);

        SetPackageLineItems(
            request,
            shipment,
            details,
            sequenceNumber);

        return request;
    }

    private DeleteShipmentRequest CreateDeleteShipmentRequest(string trackingNumber)
    {
        return new DeleteShipmentRequest
        {
            WebAuthenticationDetail = new WebAuthenticationDetail
            {
                UserCredential = new WebAuthenticationCredential
                {
                    Key = _options.FedExKey,
                    Password = _options.FedExPassword
                }
            },
            ClientDetail = new ClientDetail
            {
                AccountNumber = _options.FedExAccountNumber,
                MeterNumber = _options.FedExMeterNumber
            },
            Version = new VersionId(),
            ShipTimestamp = DateTime.Now,
            TrackingId = new TrackingId
            {
                TrackingNumber = trackingNumber,
                TrackingIdType = TrackingIdType.FEDEX,
                TrackingIdTypeSpecified = true
            },
            DeletionControl = DeletionControlType.DELETE_ALL_PACKAGES
        };
    }

    private ProcessShipmentRequest CreateProcessShipmentRequest(ShipmentDetails details)
    {
        return new ProcessShipmentRequest
        {
            WebAuthenticationDetail = new WebAuthenticationDetail
            {
                UserCredential = new WebAuthenticationCredential
                {
                    Key = _options.FedExKey,
                    Password = _options.FedExPassword
                }
            },
            ClientDetail = new ClientDetail
            {
                AccountNumber = _options.FedExAccountNumber,
                MeterNumber = _options.FedExMeterNumber
            },
            TransactionDetail = new TransactionDetail
            {
                CustomerTransactionId = details.TransactionId
            },
            Version = new VersionId()
        };
    }

    private void SetShipmentDetails(
        ProcessShipmentRequest request,
        FedExServiceType serviceType,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails details)
    {
        request.RequestedShipment = new RequestedShipment
        {
            ShipTimestamp = shipment.Options.ShippingDate,
            ServiceType = serviceType.Value == FedExServiceType.FedExInternationalPriority.Value ? "INTERNATIONAL_PRIORITY" : serviceType.Name,
            PackagingType = shipment.Options.PackagingType,
            PackageCount = shipment.Packages.Count.ToString(),

            TotalWeight = new Weight
            {
                Value = shipment.GetTotalWeight(),
                Units = WeightUnits.LB
            },

            //RateRequestTypes = details.RateRequestType.ToLower() switch
            //{
            //    "none" => new RateRequestType[1] { RateRequestType.NONE },
            //    "list" => new RateRequestType[1] { RateRequestType.LIST },
            //    "preferred" => new RateRequestType[1] { RateRequestType.PREFERRED },
            //    _ => new RateRequestType[1] { RateRequestType.NONE },
            //},

            DropoffType = shipment.Options.DropOffType.ToLower() switch
            {
                "regularpickup" => DropoffType.REGULAR_PICKUP,
                "dropbox" => DropoffType.DROP_BOX,
                "businessservicecenter" => DropoffType.BUSINESS_SERVICE_CENTER,
                "requestcourier" => DropoffType.REQUEST_COURIER,
                "station" => DropoffType.STATION,
                _ => DropoffType.REGULAR_PICKUP
            }
        };

        request.RequestedShipment.PackageCount = shipment.Packages.Count.ToString();
        request.RequestedShipment.TotalWeight = new Weight()
        {
            Units = WeightUnits.LB,
            Value = shipment.Packages.Sum(x => x.Weight)
        };

        SetSender(request, shipment, details);

        SetRecipient(request, shipment, details);

        SetPayment(request, details);

        SetLabelDetails(request, shipment, details);
    }

    private void SetSender(
        ProcessShipmentRequest request,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails details)
    {
        request.RequestedShipment.Shipper = new Party
        {
            Contact = details.Sender.Map(),
            Address = shipment.OriginAddress.GetFedExAddress()
        };
    }

    private void SetRecipient(
        ProcessShipmentRequest request,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails details)
    {
        request.RequestedShipment.Recipient = new Party
        {
            Contact = details.Recipient.Map(),
            Address = shipment.DestinationAddress.GetFedExAddress()
        };
    }

    private void SetPayment(
        ProcessShipmentRequest request,
        ShipmentDetails details)
    {
        var paymentType = details.PaymentType.Value switch
        {
            (int)PaymentType.SENDER => PaymentType.SENDER,
            (int)PaymentType.RECIPIENT => PaymentType.RECIPIENT,
            (int)PaymentType.THIRD_PARTY => PaymentType.THIRD_PARTY,
            (int)PaymentType.ACCOUNT => PaymentType.ACCOUNT,
            (int)PaymentType.COLLECT => PaymentType.COLLECT,
            _ => PaymentType.SENDER
        };

        switch (paymentType)
        {
            case PaymentType.SENDER:
                request.RequestedShipment.ShippingChargesPayment = new Payment()
                {
                    PaymentType = paymentType,
                    Payor = new Payor()
                    {
                        ResponsibleParty = new Party()
                        {
                            Contact = details.Sender.Map(),
                            AccountNumber = request.ClientDetail.AccountNumber,
                        },
                    }
                };
                break;
            case PaymentType.ACCOUNT:
            case PaymentType.COLLECT:
            case PaymentType.RECIPIENT:
            case PaymentType.THIRD_PARTY:
                request.RequestedShipment.ShippingChargesPayment = new Payment()
                {
                    PaymentType = paymentType,
                    Payor = new Payor()
                    {
                        ResponsibleParty = new Party()
                        {
                            Contact = details.Recipient.Map(),
                            AccountNumber = details.AccountNumber,
                        },
                    }
                };
                break;
        }
    }

    private void SetLabelDetails(
        ProcessShipmentRequest request,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails details)
    {
        request.RequestedShipment.LabelSpecification = new LabelSpecification
        {
            LabelFormatType = details.LabelOptions.LabelFormatType.ToUpper() switch
            {
                "COMMON2D" => LabelFormatType.COMMON2D,
                "LABEL_DATA_ONLY" => LabelFormatType.LABEL_DATA_ONLY,
                _ => LabelFormatType.COMMON2D,
            },

            ImageType = details.LabelOptions.ImageType.ToUpper() switch
            {
                "PNG" => ShippingDocumentImageType.PNG,
                "PDF" => ShippingDocumentImageType.PDF,
                _ => ShippingDocumentImageType.PDF,
            },

            ImageTypeSpecified = true,
            PrintedLabelOrigin = new ContactAndAddress()
            {
                Contact = details.Sender.Map(),
                Address = shipment.OriginAddress.GetFedExAddress()
            },

            LabelStockType = details.LabelOptions.LabelSize.ToLower() switch
            {
                "4x6" => LabelStockType.PAPER_4X6,
                "4x675" => LabelStockType.PAPER_4X675,
                "4x8" => LabelStockType.PAPER_4X8,
                "paper_letter" => LabelStockType.PAPER_LETTER,
                _ => LabelStockType.PAPER_4X6
            }
        };
    }

    private void SetPackageLineItems(
        ProcessShipmentRequest request,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails details,
        int sequenceNumber)
    {
        request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[1];

        request.RequestedShipment.RequestedPackageLineItems[0] = new RequestedPackageLineItem
        {
            SequenceNumber = (sequenceNumber + 1).ToString(),
            GroupPackageCount = "1",

            Weight = new Weight()
            {
                Units = WeightUnits.LB,
                Value = shipment.Packages[sequenceNumber].Weight,
            },

            Dimensions = new ShipClient.v25.Dimensions()
            {
                Length = shipment.Packages[sequenceNumber].Dimensions.RoundedLength.ToString(),
                Width = shipment.Packages[sequenceNumber].Dimensions.RoundedWidth.ToString(),
                Height = shipment.Packages[sequenceNumber].Dimensions.RoundedHeight.ToString(),
                Units = LinearUnits.IN
            },
            InsuredValue = new Money
            {
                Amount = shipment.Packages[sequenceNumber].InsuredValue,
                Currency = shipment.Options.GetCurrencyCode()
            },
            CustomerReferences = new CustomerReference[]
            {
                new CustomerReference
                {
                    CustomerReferenceType = details.CustomerReferenceType.ToLower() switch
                    {
                        "customer_reference" => CustomerReferenceType.CUSTOMER_REFERENCE,
                        "department_number" => CustomerReferenceType.DEPARTMENT_NUMBER,
                        "intracountry_regulatory_reference" => CustomerReferenceType.INTRACOUNTRY_REGULATORY_REFERENCE,
                        "invoice_number" => CustomerReferenceType.INVOICE_NUMBER,
                        "po_number" => CustomerReferenceType.P_O_NUMBER,
                        "rma_association" => CustomerReferenceType.RMA_ASSOCIATION,
                        "shipment_integrity" => CustomerReferenceType.SHIPMENT_INTEGRITY,
                        _ => CustomerReferenceType.CUSTOMER_REFERENCE
                    },
                    Value = request.TransactionDetail.CustomerTransactionId
                }
            }
        };

        var specialServiceTypes = new string[0];
        var signatureOptionDetail = new SignatureOptionDetail
        {
            OptionType = details.DeliverySignatureOptions.ToLower() switch
            {
                "service_default" => SignatureOptionType.SERVICE_DEFAULT,
                "adult" => SignatureOptionType.ADULT,
                "direct" => SignatureOptionType.DIRECT,
                "indirect" => SignatureOptionType.INDIRECT,
                "nosignaturerequired" => SignatureOptionType.NO_SIGNATURE_REQUIRED,
                _ => SignatureOptionType.SERVICE_DEFAULT
            }
        };

        if (shipment.Packages[sequenceNumber].SignatureRequiredOnDelivery &&
            signatureOptionDetail.OptionType == SignatureOptionType.NO_SIGNATURE_REQUIRED)
        {
            signatureOptionDetail.OptionType = SignatureOptionType.SERVICE_DEFAULT;
        }

        request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested = new PackageSpecialServicesRequested()
        {
            SignatureOptionDetail = signatureOptionDetail,
            SpecialServiceTypes = specialServiceTypes.Append("SIGNATURE_OPTION").ToArray(),
        };

        var eventTypes = new List<NotificationEventType>();

        foreach (var item in details.EmailNotification.EmailNotificationTypes)
        {
            eventTypes.Add(item.Name.ToEnum<NotificationEventType>());
        }

        request.RequestedShipment.SpecialServicesRequested = new ShipmentSpecialServicesRequested
        {
            SpecialServiceTypes = specialServiceTypes.Append("EVENT_NOTIFICATION").ToArray(),

            EventNotificationDetail = new ShipmentEventNotificationDetail
            {
                // notification event message
                PersonalMessage = details.EmailNotification.PersonalMessage,
                EventNotifications = new ShipmentEventNotificationSpecification[]
                {
                    new ShipmentEventNotificationSpecification
                    {
                        Events = eventTypes.ToArray(),
                        NotificationDetail = new NotificationDetail
                        {
                            NotificationType = NotificationType.EMAIL,
                            EmailDetail = new EMailDetail()
                            {
                                EmailAddress = details.Recipient.Email,
                                Name = details.Recipient.FullName,
                            },
                            Localization = new Localization
                            {
                                LanguageCode = details.EmailNotification.LanguageCode
                            }
                        },
                        FormatSpecification = new ShipmentNotificationFormatSpecification
                        {
                            Type = details.EmailNotification.NotificationFormatType.ToLower() switch
                            {
                                "html" => NotificationFormatType.HTML,
                                "text" => NotificationFormatType.TEXT,
                                _ => NotificationFormatType.HTML
                            }
                        },
                        Role = details.EmailNotification.NotificationRoleType.ToLower() switch
                        {
                                "recipient" => ShipmentNotificationRoleType.RECIPIENT,
                                "broker" => ShipmentNotificationRoleType.BROKER,
                                "shipper" => ShipmentNotificationRoleType.SHIPPER,
                                "third_party" => ShipmentNotificationRoleType.THIRD_PARTY,
                                "other" => ShipmentNotificationRoleType.OTHER,
                                _ => ShipmentNotificationRoleType.RECIPIENT
                        },
                        RoleSpecified = true
                    }
                },
            }
        };

        if (!shipment.DestinationAddress.IsUnitedStatesAddress() || shipment.DestinationAddress.IsUnitedStatesTerritory())
        {
            if (details.LabelOptions.EnableEtd)
            {
                request.RequestedShipment.EdtRequestType = EdtRequestType.ALL;
                request.RequestedShipment.EdtRequestTypeSpecified = true;

                request.RequestedShipment.SpecialServicesRequested = new ShipmentSpecialServicesRequested
                {
                    SpecialServiceTypes = specialServiceTypes.Append("ELECTRONIC_TRADE_DOCUMENTS").ToArray(),
                    EtdDetail = new EtdDetail
                    {
                        RequestedDocumentCopies = new RequestedShippingDocumentType[]
                        {
                            RequestedShippingDocumentType.COMMERCIAL_INVOICE
                        }
                    }
                };
            }

            request.RequestedShipment.CustomsClearanceDetail = new CustomsClearanceDetail
            {
                ExportDetail = GetExportDetail(shipment, details),
                CommercialInvoice = new CommercialInvoice
                {
                    CustomerReferences = new CustomerReference[]
                    {
                        new CustomerReference
                        {
                            CustomerReferenceType = details.CustomerReferenceType.ToLower() switch
                            {
                                "customer_reference" => CustomerReferenceType.CUSTOMER_REFERENCE,
                                "department_number" => CustomerReferenceType.DEPARTMENT_NUMBER,
                                "intracountry_regulatory_reference" => CustomerReferenceType.INTRACOUNTRY_REGULATORY_REFERENCE,
                                "invoice_number" => CustomerReferenceType.INVOICE_NUMBER,
                                "po_number" => CustomerReferenceType.P_O_NUMBER,
                                "rma_association" => CustomerReferenceType.RMA_ASSOCIATION,
                                "shipment_integrity" => CustomerReferenceType.SHIPMENT_INTEGRITY,
                                _ => CustomerReferenceType.CUSTOMER_REFERENCE
                            },
                            Value = request.TransactionDetail.CustomerTransactionId
                        }
                    }
                },
                CustomsValue = new Money()
                {
                    Amount = details.Commodities.Sum(x => x.CustomsValue),
                    Currency = shipment.Options.GetCurrencyCode(),
                },
                PartiesToTransactionAreRelated = false,
                PartiesToTransactionAreRelatedSpecified = true,
                DutiesPayment = new Payment()
                {
                    PaymentType = PaymentType.RECIPIENT
                }
            };

            request.RequestedShipment.ShippingDocumentSpecification = new ShippingDocumentSpecification
            {
                ShippingDocumentTypes = new RequestedShippingDocumentType[0],
                CommercialInvoiceDetail = new CommercialInvoiceDetail()
                {
                    Format = new ShippingDocumentFormat()
                    {
                        ImageType = ShippingDocumentImageType.PDF,
                        ImageTypeSpecified = true,
                        StockType = ShippingDocumentStockType.PAPER_LETTER,
                        StockTypeSpecified = true
                    },
                    CustomerImageUsages = new CustomerImageUsage[2]
                    {
                        new CustomerImageUsage()
                        {
                            Type = CustomerImageUsageType.LETTER_HEAD,
                            TypeSpecified = true,
                            Id = (ImageId)details.LabelOptions.LetterHeadImageId,
                            IdSpecified = details.LabelOptions.EnableEtd
                        },
                        new CustomerImageUsage()
                        {
                            Type = CustomerImageUsageType.SIGNATURE,
                            TypeSpecified = true,
                            Id = (ImageId)details.LabelOptions.SignatureImageId,
                            IdSpecified = details.LabelOptions.EnableEtd
                        }
                    }
                }
            };
            var documentTypes = new List<RequestedShippingDocumentType>();

            foreach (var docType in details.RequestedDocumentTypes)
            {
                documentTypes.Add(
                    docType.Value switch
                    {
                        (int)RequestedShippingDocumentType.COMMERCIAL_INVOICE => RequestedShippingDocumentType.COMMERCIAL_INVOICE,
                        (int)RequestedShippingDocumentType.PRO_FORMA_INVOICE => RequestedShippingDocumentType.PRO_FORMA_INVOICE,
                        (int)RequestedShippingDocumentType.CERTIFICATE_OF_ORIGIN => RequestedShippingDocumentType.CERTIFICATE_OF_ORIGIN,
                        _ => RequestedShippingDocumentType.COMMERCIAL_INVOICE
                    });

                documentTypes.Distinct();

                if (docType == FedExRequestedDocumentType.CertificateOfOrigin)
                {
                    request.RequestedShipment.ShippingDocumentSpecification.CertificateOfOrigin = new CertificateOfOriginDetail()
                    {
                        DocumentFormat = new ShippingDocumentFormat()
                        {
                            ImageType = ShippingDocumentImageType.PDF,
                            ImageTypeSpecified = true,
                            StockType = ShippingDocumentStockType.PAPER_LETTER,
                            StockTypeSpecified = true
                        }
                    };
                }
            }

            var totalValue = details.Commodities.Sum(x => x.CustomsValue);

            if ((shipment.DestinationAddress.IsCanadaAddress() && totalValue <= 3300m) ||
                (shipment.DestinationAddress.IsMexicoAddress() && totalValue <= 1000m))
            {
                request.RequestedShipment.CustomsClearanceDetail.CommercialInvoice.SpecialInstructions = "Simplified Low Value Certification/Statement (LVS): I hereby certify that the goods covered by this shipment qualify as an originating good for the purposes of preferential tariff treatment under USMCA/T-MEC/CUSMA";
            }

            request.RequestedShipment.ShippingDocumentSpecification.ShippingDocumentTypes = documentTypes.ToArray();

            var commodities = new List<ShipClient.v25.Commodity>();

            foreach (var commodity in details.Commodities)
            {
                var commodityInstance = new ShipClient.v25.Commodity()
                {
                    Description = !string.IsNullOrEmpty(commodity.Description) ? commodity.Description : string.Empty,

                    Name = !string.IsNullOrEmpty(commodity.Name) ? commodity.Name : string.Empty,

                    NumberOfPieces = !string.IsNullOrEmpty(commodity.NumberOfPieces.ToString()) ? commodity.NumberOfPieces.ToString() : string.Empty,

                    CountryOfManufacture = !string.IsNullOrEmpty(commodity.CountryOfManufacturer) ? commodity.CountryOfManufacturer : string.Empty,

                    Weight = new Weight()
                    {
                        Units = WeightUnits.LB,
                        Value = commodity.Weight
                    },

                    Quantity = commodity.Quantity,

                    QuantityUnits = !string.IsNullOrEmpty(commodity.QuantityUnits) ? commodity.QuantityUnits : string.Empty,

                    UnitPrice = new Money()
                    {
                        Amount = commodity.UnitPrice,
                        Currency = shipment.Options.GetCurrencyCode()
                    },

                    HarmonizedCode = !string.IsNullOrEmpty(commodity.HarmonizedCode) ? commodity.HarmonizedCode : string.Empty,

                    ExportLicenseNumber = !string.IsNullOrEmpty(commodity.ExportLicenseNumber) ? commodity.ExportLicenseNumber : string.Empty,

                    ExportLicenseExpirationDate = commodity.ExportLicenseExpirationDate,

                    PartNumber = !string.IsNullOrEmpty(commodity.PartNumber) ? commodity.PartNumber : string.Empty,

                    Purpose = commodity.Purpose.ToUpper() switch
                    {
                        "BUSINESS" => CommodityPurposeType.BUSINESS,
                        "CONSUMER" => CommodityPurposeType.CONSUMER,
                        _ => CommodityPurposeType.BUSINESS
                    },
                    PurposeSpecified = true,

                    CustomsValue = new Money()
                    {
                        Amount = commodity.CustomsValue,
                        Currency = shipment.Options.GetCurrencyCode()
                    },

                    CIMarksAndNumbers = commodity.CIMarksandNumbers,

                    QuantitySpecified = commodity.Quantity > 0,

                    ExportLicenseExpirationDateSpecified = commodity.ExportLicenseExpirationDate != default,
                };

                commodities.Add(commodityInstance);
            }

            request.RequestedShipment.CustomsClearanceDetail.Commodities = commodities.ToArray();
        }

        if (details?.CollectOnDelivery != null)
        {
            request.RequestedShipment.SpecialServicesRequested = new ShipmentSpecialServicesRequested
            {
                SpecialServiceTypes = specialServiceTypes.Append("COD").ToArray(),

                CodDetail = new CodDetail()
                {
                    CodCollectionAmount = new Money()
                    {
                        Amount = details.CollectOnDelivery.Amount,
                        Currency = shipment.Options.PreferredCurrencyCode
                    },

                    CollectionType = details.CollectOnDelivery.CollectionType.Value switch
                    {
                        (int)CodCollectionType.GUARANTEED_FUNDS => CodCollectionType.GUARANTEED_FUNDS,
                        (int)CodCollectionType.CASH => CodCollectionType.CASH,
                        (int)CodCollectionType.ANY => CodCollectionType.ANY,
                        (int)CodCollectionType.COMPANY_CHECK => CodCollectionType.COMPANY_CHECK,
                        _ => CodCollectionType.GUARANTEED_FUNDS
                    }
                }
            };
        }
    }

    private ExportDetail? GetExportDetail(Shipping.Abstractions.Models.Shipment shipment, ShipmentDetails details)
    {
        if (shipment.OriginAddress.IsUnitedStatesAddress() || shipment.OriginAddress.IsCanadaAddress())
        {
            return new ExportDetail()
            {
                B13AFilingOption = (B13AFilingOptionType)details.ExportDetails.B13AFilingOption,
                B13AFilingOptionSpecified = true,
                ExportComplianceStatement = details.ExportDetails.ComplianceStatement
            };
        }

        return null;
    }
}
