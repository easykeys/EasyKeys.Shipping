using EasyKeys.Shipping.Abstractions.Extensions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;
using EasyKeys.Shipping.FedEx.Shipment.Extensions;
using EasyKeys.Shipping.FedEx.Shipment.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ShipClient.v25;

namespace EasyKeys.Shipping.FedEx.Shipment;

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

                shipmentRequest.ProcessShipmentRequest.RequestedShipment.MasterTrackingId = masterTrackingId == default(TrackingId) ? null : masterTrackingId;

                var response = await client.processShipmentAsync(shipmentRequest);

                var reply = response?.ProcessShipmentReply;

                if ((reply?.HighestSeverity != NotificationSeverityType.ERROR)
                         && (reply?.HighestSeverity != NotificationSeverityType.FAILURE))
                {
                    var charges = new PackageCharges();

                    var packageDetails = reply?.CompletedShipmentDetail?.CompletedPackageDetails?.ToList();

                    var totalShipmentDetails = reply?.CompletedShipmentDetail.ShipmentRating?.ShipmentRateDetails.ToList();
                    reply?.CompletedShipmentDetail.CompletedPackageDetails.ToList().
                        ForEach(x =>
                        {
                            charges.Surcharges = new Dictionary<string, decimal>();

                            // if shipment destination is international
                            if (reply?.CompletedShipmentDetail.ShipmentRating != null)
                            {
                                charges.BaseCharge = totalShipmentDetails.Select(x => x.TotalBaseCharge).Sum(x => x.Amount);
                                charges.NetCharge = totalShipmentDetails.Select(x => x.TotalNetCharge).Sum(x => x.Amount);
                                charges.TotalSurCharges = totalShipmentDetails.Select(x => x.TotalSurcharges).Sum(x => x.Amount);
                                totalShipmentDetails.SelectMany(x => x.Surcharges)
                                                        .ToList()
                                                        .ForEach(x => charges.Surcharges[x.Description] = x.Amount.Amount);
                            }

                            // if shipment destination is domestic
                            if (x.PackageRating != null)
                            {
                                charges.BaseCharge = x.PackageRating.PackageRateDetails.Select(x => x.BaseCharge).Sum(s => s.Amount);
                                charges.NetCharge = x.PackageRating.PackageRateDetails.Select(x => x.NetCharge).Sum(s => s.Amount);

                                x.PackageRating.PackageRateDetails
                                                    .SelectMany(x => x.Surcharges)
                                                    .ToList()
                                                    .ForEach(x => charges.Surcharges[x.Description] = x.Amount.Amount);

                                charges.TotalSurCharges = x.PackageRating.PackageRateDetails.Select(x => x.TotalSurcharges).Sum(s => s.Amount);
                            }

                            label.Labels.Add(
                            new PackageLabelDetails
                            {
                                Charges = charges,

                                TrackingId = x.TrackingIds.Select(x => x.TrackingNumber).Flatten(";"),

                                ImageType = x.Label.ImageType.ToString(),

                                Bytes = x.Label.Parts.Select(x => x.Image).ToList()
                            });
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

            // this does not explain fault exceptions well, must debug handler
            // TODO: possible needed to have Inner Exception support added.
            label.InternalErrors.Add(ex?.Message ?? $"{nameof(FedExShipmentProvider)} failed");
        }

        return label;
    }

    private ProcessShipmentRequest CreateShipmentRequest(
        FedExServiceType serviceType,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails details,
        int sequenceNumber)
    {
        var request = CreateRequest(details);

        SetShipmentDetails(
            request,
            serviceType,
            shipment,
            details);

        SetpackageLineItems(
            request,
            shipment,
            details,
            sequenceNumber);

        return request;
    }

    private ProcessShipmentRequest CreateRequest(ShipmentDetails details)
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
            ServiceType = serviceType.Name,
            PackagingType = shipment.Options.PackagingType,
            PackageCount = shipment.Packages.Count.ToString(),
            TotalWeight = new Weight
            {
                Value = shipment.Packages.Sum(x => x.Weight),
                Units = WeightUnits.LB
            },

            RateRequestTypes = details.RateRequestType.ToLower() switch
            {
                "none" => new RateRequestType[1] { RateRequestType.NONE },
                "list" => new RateRequestType[1] { RateRequestType.LIST },
                "preferred" => new RateRequestType[1] { RateRequestType.PREFERRED },
                _ => throw new NotImplementedException(),
            }
        };

        request.RequestedShipment.DropoffType = shipment.Options.DropOffType.ToLower() switch
        {
            "regularpickup" => DropoffType.REGULAR_PICKUP,
            "dropbox" => DropoffType.DROP_BOX,
            "businessservicecenter" => DropoffType.BUSINESS_SERVICE_CENTER,
            "requestcourier" => DropoffType.REQUEST_COURIER,
            "station" => DropoffType.STATION,
            _ => DropoffType.REGULAR_PICKUP
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
        var paymentType = details.PaymentType.ToLower() switch
        {
            "sender" => PaymentType.SENDER,
            "recipient" => PaymentType.RECIPIENT,
            "third_party" => PaymentType.THIRD_PARTY,
            "account" => PaymentType.ACCOUNT,
            "collect" => PaymentType.COLLECT,
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

    private void SetpackageLineItems(
        ProcessShipmentRequest request,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails details,
        int sequenceNumber)
    {
        request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[1];

        request.RequestedShipment.RequestedPackageLineItems[0] = new RequestedPackageLineItem()
        {
            SequenceNumber = (sequenceNumber + 1).ToString(),
            GroupPackageCount = "1",

            Weight = new Weight()
            {
                Units = WeightUnits.LB,
                Value = shipment.Packages[sequenceNumber].RoundedWeight,
            },

            Dimensions = new ShipClient.v25.Dimensions()
            {
                Length = shipment.Packages[sequenceNumber].Dimensions.RoundedLength.ToString(),
                Width = shipment.Packages[sequenceNumber].Dimensions.RoundedWidth.ToString(),
                Height = shipment.Packages[sequenceNumber].Dimensions.RoundedHeight.ToString(),
                Units = LinearUnits.IN
            }
        };
        request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue = new Money
        {
            Amount = shipment.Packages[sequenceNumber].InsuredValue,
            Currency = shipment.Options.GetCurrencyCode()
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
            SpecialServiceTypes = specialServiceTypes.Append("SIGNATURE_OPTION").ToArray()
        };

        var eventTypes = new NotificationEventType[0];

        details.EmailNotification.EmailNotificationTypes.ToList().ForEach(x =>
        {
            _ = x switch
            {
                "On_Shipment" => eventTypes.Append(NotificationEventType.ON_SHIPMENT),
                "On_Delivery" => eventTypes.Append(NotificationEventType.ON_DELIVERY),
                "On_Estimated_Delivery" => eventTypes.Append(NotificationEventType.ON_ESTIMATED_DELIVERY),
                "On_Exception" => eventTypes.Append(NotificationEventType.ON_EXCEPTION),
                "On_Pickup_Driver_Arrived" => eventTypes.Append(NotificationEventType.ON_PICKUP_DRIVER_ARRIVED),
                "On_Pickup_Driver_Assigned" => eventTypes.Append(NotificationEventType.ON_PICKUP_DRIVER_ASSIGNED),
                "On_Pickup_Driver_Departed" => eventTypes.Append(NotificationEventType.ON_PICKUP_DRIVER_DEPARTED),
                "On_Pickup_Driver_In_Route" => eventTypes.Append(NotificationEventType.ON_PICKUP_DRIVER_EN_ROUTE),
                "On_Tender" => eventTypes.Append(NotificationEventType.ON_TENDER),
                _ => default
            };
        });

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

        request.RequestedShipment.CustomsClearanceDetail = new CustomsClearanceDetail
        {
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
            }
        };

        if (shipment.DestinationAddress.CountryCode != "US")
        {
            request.RequestedShipment.CustomsClearanceDetail.CustomsValue = new Money()
            {
                Amount = details.Commodities.Sum(x => x.UnitPrice),
                Currency = shipment.Options.GetCurrencyCode()
            };

            request.RequestedShipment.CustomsClearanceDetail.DutiesPayment = request.RequestedShipment.ShippingChargesPayment;

            foreach (var commodity in details.Commodities)
            {
                request.RequestedShipment.CustomsClearanceDetail.Commodities = new ShipClient.v25.Commodity[]
                {
                    new ShipClient.v25.Commodity()
                    {
                        Description = commodity.Description,

                        Name = commodity.Name,

                        NumberOfPieces = commodity.NumberOfPieces.ToString(),

                        CountryOfManufacture = commodity.CountryOfManufacturer,

                        Weight = new Weight()
                            {
                                Units = WeightUnits.LB,
                                Value = shipment.GetTotalWeight(),
                            },

                        Quantity = commodity.Quantity,

                        QuantityUnits = commodity.QuantityUnits,

                        UnitPrice = new Money()
                            {
                                Amount = commodity.UnitPrice,
                                Currency = shipment.Options.GetCurrencyCode()
                            },

                        HarmonizedCode = commodity.HarmonizedCode,

                        ExportLicenseNumber = commodity.ExportLicenseNumber,

                        ExportLicenseExpirationDate = commodity.ExportLicenseExpirationDate,

                        PartNumber = commodity.PartNumber,

                        Purpose = commodity.Purpose.ToUpper() switch
                        {
                            "BUSINESS" => CommodityPurposeType.BUSINESS,
                            "CONSUMER" => CommodityPurposeType.CONSUMER,
                            _ => CommodityPurposeType.BUSINESS
                        },
                        CustomsValue = new Money() { Amount = commodity.CustomsValue, Currency = shipment.Options.GetCurrencyCode() },

                        CIMarksAndNumbers = commodity.CIMarksandNumbers,

                        PurposeSpecified = true,

                        QuantitySpecified = true,

                        ExportLicenseExpirationDateSpecified = true
                    }
                };
            }
        }

        if (details.CollectOnDelivery.Activated)
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
                    CollectionType = details.CollectOnDelivery.CollectionType.ToUpper() switch
                    {
                        "GUARANTEED_FUNDS" => CodCollectionType.GUARANTEED_FUNDS,
                        "CASH" => CodCollectionType.CASH,
                        "ANY" => CodCollectionType.ANY,
                        "COMPANY_CHECK" => CodCollectionType.COMPANY_CHECK,
                        _ => CodCollectionType.GUARANTEED_FUNDS
                    }
                }
            };
        }
    }
}
