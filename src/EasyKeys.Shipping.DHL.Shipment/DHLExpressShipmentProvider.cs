using System.Text;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.DHL.Abstractions;
using EasyKeys.Shipping.DHL.Abstractions.OpenApis.V2.Express;
using EasyKeys.Shipping.DHL.Abstractions.Options;
using EasyKeys.Shipping.DHL.Shipment.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyKeys.Shipping.DHL.Shipment;

public class DHLExpressShipmentProvider : IDHLExpressShipmentProvider
{
    private readonly DHLExpressApi _dHLExpressApi;
    private readonly IPaperlessEligibilityService _paperlessEligibilityService;
    private readonly DHLExpressApiOptions _dHLExpressApiOptions;
    private readonly ILogger<DHLExpressShipmentProvider> _logger;

    public DHLExpressShipmentProvider(
        IOptionsMonitor<DHLExpressApiOptions> dHLExpressApiOptions,
        IPaperlessEligibilityService paperlessEligibilityService,
        DHLExpressApi dHLExpressApi,
        ILogger<DHLExpressShipmentProvider> logger)
    {
        _paperlessEligibilityService = paperlessEligibilityService ?? throw new ArgumentNullException(nameof(paperlessEligibilityService));
        _dHLExpressApiOptions = dHLExpressApiOptions?.CurrentValue ?? throw new ArgumentNullException(nameof(dHLExpressApiOptions));
        _dHLExpressApi = dHLExpressApi ?? throw new ArgumentNullException(nameof(dHLExpressApi));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ShipmentLabel> CreateShipmentAsync(Shipping.Abstractions.Models.Shipment shipment, ShippingDetails details, CancellationToken cancellationToken = default)
    {
        var label = new ShipmentLabel();
        try
        {
            var body = new SupermodelIoLogisticsExpressCreateShipmentRequest
            {
                PlannedShippingDateAndTime = shipment.Options.ShippingDate.ToString("yyyy-MM-dd'T'HH:mm:ss") + " GMT+00:00",
                Pickup = new Pickup { IsRequested = details.IsPickupRequested },
                ProductCode = details.ProductCode,
                LocalProductCode = details.ProductCode,
                GetRateEstimates = true,
                Accounts = new[]
                {
                    new SupermodelIoLogisticsExpressAccount
                    {
                        Number = _dHLExpressApiOptions.AccountNumber,
                        TypeCode = SupermodelIoLogisticsExpressAccountTypeCode.Shipper
                    }
                },

                OutputImageProperties = new OutputImageProperties
                {
                    CustomerLogos = details.Logos.Select(x => new CustomerLogos
                    {
                        Content = x.Content,
                        FileFormat = (CustomerLogosFileFormat)x.FileFormat,
                    }).ToArray(),
                    PrinterDPI = PrinterDPI.DPI300,
                    EncodingFormat = OutputImagePropertiesEncodingFormat.Pdf,
                    ImageOptions = new[]
                    {
                        new ImageOptions
                        {
                            TypeCode = ImageOptionsTypeCode.Invoice,
                            TemplateName = "COMMERCIAL_INVOICE_P_10",
                            IsRequested = true,
                            InvoiceType = ImageOptionsInvoiceType.Commercial,
                            LanguageCode = "eng",
                            LanguageCountryCode = "US"
                        },
                        new ImageOptions
                        {
                            TypeCode = ImageOptionsTypeCode.WaybillDoc,
                            TemplateName = "ARCH_8x4",
                            IsRequested = true,
                            HideAccountNumber = false,
                            NumberOfCopies = 1
                        },
                        new ImageOptions
                        {
                            TypeCode = ImageOptionsTypeCode.Label,
                            TemplateName = "ECOM26_84_001",
                            RenderDHLLogo = true,
                            FitLabelsToA4 = false
                        }
                    },
                    SplitTransportAndWaybillDocLabels = details.SplitTransportAndWaybillDocLabls,
                    AllDocumentsInOneImage = details.AllDocumentsInOneImage,
                    SplitDocumentsByPages = details.SplitDocumentsByPages,
                    SplitInvoiceAndReceipt = details.SplitInvoiceAndReceipt,
                    ReceiptAndLabelsInOneImage = details.ReceiptAndLabelsInOneImage
                },
                CustomerDetails = new CustomerDetails
                {
                    ReceiverDetails = new ReceiverDetails
                    {
                        PostalAddress = new SupermodelIoLogisticsExpressAddressCreateShipmentRequest
                        {
                            PostalCode = shipment.DestinationAddress.PostalCode,
                            CityName = shipment.DestinationAddress.City,
                            CountryCode = shipment.DestinationAddress.CountryCode,
                            AddressLine1 = shipment.DestinationAddress.StreetLine,
                            AddressLine2 = string.IsNullOrEmpty(shipment.DestinationAddress.StreetLine2) ? null : shipment.DestinationAddress.StreetLine2,
                            AddressLine3 = shipment.DestinationAddress.StateOrProvince
                        },
                        ContactInformation = new SupermodelIoLogisticsExpressContact
                        {
                            Email = details.Recipient.Email,
                            Phone = details.Recipient.PhoneNumber,
                            CompanyName = details.Recipient.Company,
                            FullName = details.Recipient.FullName
                        }
                    },
                    ShipperDetails = new ShipperDetails
                    {
                        PostalAddress = new SupermodelIoLogisticsExpressAddressCreateShipmentRequest
                        {
                            PostalCode = shipment.OriginAddress.PostalCode,
                            CityName = shipment.OriginAddress.City,
                            CountryCode = shipment.OriginAddress.CountryCode,
                            AddressLine1 = shipment.OriginAddress.StreetLine,
                            AddressLine2 = string.IsNullOrEmpty(shipment.OriginAddress.StreetLine2) ? null : shipment.OriginAddress.StreetLine2,
                            AddressLine3 = shipment.OriginAddress.StateOrProvince
                        },
                        ContactInformation = new SupermodelIoLogisticsExpressContact
                        {
                            Email = details.Sender.Email,
                            Phone = details.Sender.PhoneNumber,
                            CompanyName = details.Sender.Company,
                            FullName = details.Sender.FullName
                        },
                        RegistrationNumbers = [new SupermodelIoLogisticsExpressRegistrationNumbers
                        {
                            IssuerCountryCode = shipment.OriginAddress.CountryCode,
                            Number = details.Sender.TaxId,
                            TypeCode = SupermodelIoLogisticsExpressRegistrationNumbersTypeCode.VAT
                        }

                        ],
                        TypeCode = ShipperDetailsTypeCode.Business
                    }
                },
                Content = new Content2
                {
                    USFilingTypeValue = GetFilingType(shipment, details),
                    IsCustomsDeclarable = true,
                    DeclaredValue = (double)details.Commodities.Sum(x => x.Quantity * x.UnitPrice),
                    DeclaredValueCurrency = "USD",
                    Description = "Shipment",
                    Incoterm = Content2Incoterm.DAP,
                    UnitOfMeasurement = Content2UnitOfMeasurement.Imperial,
                    Packages = shipment.Packages.Select(x =>
                    {
                        return new SupermodelIoLogisticsExpressPackage
                        {
                            Weight = (double)x.RoundedWeight,
                            Dimensions = new Dimensions2
                            {
                                Length = (double)x.Dimensions.Length,
                                Width = (double)x.Dimensions.Width,
                                Height = (double)x.Dimensions.Height
                            },
                            CustomerReferences = new[]
                            {
                                new SupermodelIoLogisticsExpressPackageReference
                                {
                                    TypeCode = SupermodelIoLogisticsExpressPackageReferenceTypeCode.CU,
                                    Value = details.InvoiceNumber
                                }
                            },
                            Description = details.PackageDescription,
                            LabelDescription = details.LabelDescription
                        };
                    }).ToArray(),
                    ExportDeclaration = new ExportDeclaration
                    {
                        LineItems = details.Commodities.Select((x, i) =>
                        {
                            return new LineItems2
                            {
                                Number = i + 1,
                                Description = x.Description,
                                Price = (double)x.UnitPrice,
                                Quantity = new Quantity2
                                {
                                    Value = x.Quantity,
                                    UnitOfMeasurement = Quantity2UnitOfMeasurement.EA
                                },
                                CommodityCodes = new[]
                                {
                                    new CommodityCodes2
                                    {
                                        TypeCode = CommodityCodes2TypeCode.Outbound,
                                        Value = x.HarmonizedCode
                                    }
                                },
                                ExportReasonType = LineItems2ExportReasonType.Commercial_purpose_or_sale,
                                ManufacturerCountry = "US",
                                Weight = new Weight4
                                {
                                    NetValue = (double)Math.Round(x.Weight, 3),
                                    GrossValue = (double)Math.Round(x.Weight, 3)
                                },
                                IsTaxesPaid = false,
                                AdditionalInformation = new[] { x.Comments },
                                CustomerReferences = new[]
                                {
                                    new CustomerReferences3
                                    {
                                        TypeCode = CustomerReferences3TypeCode.PAN,
                                        Value = x.PartNumber
                                    }
                                }
                            };
                        }).ToArray(),

                        Invoice = new Invoice2
                        {
                            Number = details.InvoiceNumber,
                            Date = DateTimeOffset.Now.Date,
                            Instructions = new[] { "Handle with care" },
                            TotalNetWeight = (double)shipment.Packages.Sum(x => x.RoundedWeight),
                            TotalGrossWeight = (double)shipment.Packages.Sum(x => x.RoundedWeight),
                            SignatureImage = details.Signature?.Content,
                            SignatureName = details.Signature?.SignatureName,
                            SignatureTitle = details.Signature?.SignatureTitle,
                            CustomerReferences = new[]
                            {
                                new CustomerReferences4 { TypeCode = CustomerReferences4TypeCode.PON, Value = details.InvoiceNumber},
                            }
                        },
                        Remarks = new[] { new Remarks2 { Value = "Right side up only" } },
                        RecipientReference = details.InvoiceNumber,
                        ExportReasonType = ExportDeclarationExportReasonType.Commercial_purpose_or_sale,
                        ShipmentType = ExportDeclarationShipmentType.Commercial
                    }
                },
                ShipmentNotification = new[]
                {
                    new ShipmentNotification
                    {
                        TypeCode = ShipmentNotificationTypeCode.Email,
                        ReceiverId = details.Recipient.Email,
                        LanguageCode = "eng",
                        LanguageCountryCode = "UK",
                        BespokeMessage = details.CustomShipmentMessage ?? "Your Package has shipped!"
                    }
                },
                EstimatedDeliveryDate = new EstimatedDeliveryDate
                {
                    IsRequested = true,
                    TypeCode = EstimatedDeliveryDateTypeCode.QDDC
                },
                GetAdditionalInformation = new[]
                {
                    new GetAdditionalInformation
                    {
                        TypeCode = GetAdditionalInformationTypeCode.PickupDetails,
                        IsRequested = false
                    }
                },
                GetTransliteratedResponse = false
            };

            body.ValueAddedServices = AddServices(shipment, details);

            var result = await _dHLExpressApi.ExpApiShipmentsAsync(body, cancellationToken: cancellationToken);

            var billCharges = result.ShipmentCharges.First(x => x.CurrencyType == "BILLC");
            var nonDiscountCharges = result.ShipmentCharges.First(x => x.CurrencyType == "PULCL");
            foreach (var doc in result.Documents.Where(x => x.TypeCode == ImageOptionsTypeCode.Label.ToString().ToLower()))
            {
                label.Labels.Add(new PackageLabelDetails
                {
                    Bytes = new () { Convert.FromBase64String(doc.Content) },
                    ImageType = doc.ImageFormat,
                    TotalCharges = new Shipping.Abstractions.Models.ShipmentCharges
                    {
                        NetCharge = (decimal)billCharges.Price,
                        SurchargesList = billCharges.ServiceBreakdown.Where(x => x.TypeCode != null).ToDictionary(x => x.Name, x => (decimal)x.Price),
                        Surcharges = (decimal)billCharges.ServiceBreakdown.Where(x => x.TypeCode != null).Sum(x => x.Price),
                        BaseCharge = (decimal)billCharges.ServiceBreakdown.Where(x => x.TypeCode == null).Sum(x => x.Price)
                    },
                    TotalCharges2 = new Shipping.Abstractions.Models.ShipmentCharges
                    {
                        NetCharge = (decimal)nonDiscountCharges.Price,
                        SurchargesList = nonDiscountCharges.ServiceBreakdown.Where(x => x.TypeCode != null).ToDictionary(x => x.Name, x => (decimal)x.Price),
                        Surcharges = (decimal)nonDiscountCharges.ServiceBreakdown.Where(x => x.TypeCode != null).Sum(x => x.Price),
                        BaseCharge = (decimal)nonDiscountCharges.ServiceBreakdown.Where(x => x.TypeCode == null).Sum(x => x.Price)
                    },
                    ProviderLabelId = result.TrackingUrl,
                    TrackingId = result.ShipmentTrackingNumber
                });
            }

            foreach (var doc in result.Documents.Where(x => x.TypeCode == ImageOptionsTypeCode.Invoice.ToString().ToLower()))
            {
                label.ShippingDocuments.Add(new Document()
                {
                    Bytes = new () { Convert.FromBase64String(doc.Content) },
                    ImageType = doc.ImageFormat,
                    DocumentName = doc.TypeCode,
                    CopiesToPrint = "1"
                });
            }
        }
        catch (ApiException<SupermodelIoLogisticsExpressErrorResponse> ex)
        {
            var error = ex?.Result.Detail ?? string.Empty;
            if (ex?.Result?.AdditionalDetails?.Any() ?? false)
            {
                error += string.Join(",", ex.Result.AdditionalDetails);
            }

            _logger.LogError("{name} : {message}", nameof(DHLExpressShipmentProvider), error);
            label.InternalErrors.Add(error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{providerName} failed", nameof(DHLExpressShipmentProvider));
            label.InternalErrors.Add(ex?.Message ?? $"{nameof(DHLExpressShipmentProvider)} failed");
        }

        return label;
    }

    private string GetFilingType(Shipping.Abstractions.Models.Shipment shipment, ShippingDetails details)
    {
        var maxValue = details.Commodities.Max(x => x.Quantity * x.UnitPrice);

        return shipment.DestinationAddress.IsCanadaAddress() ? "FTR: 30.36" : "FTR: 30.37(a)";
    }

    private SupermodelIoLogisticsExpressValueAddedServices[]? AddServices(Shipping.Abstractions.Models.Shipment shipment, ShippingDetails details)
    {
        var valueAddedServices = details.AddedServices.Select(x =>
        {
            return new SupermodelIoLogisticsExpressValueAddedServices
            {
                ServiceCode = x.Key,
                Value = x.Value,
                Currency = x.Value == null ? null : "USD"
            };
        }).ToList();

        if (shipment.Packages.Any(x => x.SignatureRequiredOnDelivery))
        {
            _logger.LogInformation("{providerName} : Adding Signature value", nameof(DHLExpressShipmentProvider));
            valueAddedServices.Add(new SupermodelIoLogisticsExpressValueAddedServices
            {
                ServiceCode = "SF",
                Value = null,
                Currency = null
            });
        }

        if (shipment.Packages.Any(x => x.InsuredValue > 0m))
        {
            _logger.LogInformation("{providerName} : Adding Insurance value of {value}", nameof(DHLExpressShipmentProvider), shipment.Packages.First(x => x.InsuredValue > 0m).InsuredValue);

            valueAddedServices.Add(new SupermodelIoLogisticsExpressValueAddedServices
            {
                ServiceCode = "II",
                Value = (double)shipment.Packages.First(x => x.InsuredValue > 0m).InsuredValue,
                Currency = "USD"
            });
        }

        if (!valueAddedServices.Any(x => x.ServiceCode == "WY")
            && _paperlessEligibilityService.IsPaperlessAvailable(shipment.DestinationAddress.CountryCode, (double)details.Commodities.Sum(x => x.Quantity * x.UnitPrice)))
        {
            _logger.LogInformation("{providerName} : Paperless eligible. Auto adding..", nameof(DHLExpressShipmentProvider));
            var paperless = new SupermodelIoLogisticsExpressValueAddedServices
            {
                ServiceCode = "WY"
            };

            valueAddedServices.Add(paperless);
        }

        return valueAddedServices.Any() ? valueAddedServices.Distinct().ToArray() : null;
    }
}
