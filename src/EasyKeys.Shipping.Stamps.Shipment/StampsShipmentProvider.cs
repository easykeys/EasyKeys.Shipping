using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.Shipment.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment
{
    public class StampsShipmentProvider : IStampsShipmentProvider
    {
        private readonly IStampsClientService _stampsClient;
        private readonly IRatesService _ratesService;

        public StampsShipmentProvider(IStampsClientService stampsClientService, IRatesService ratesService)
        {
            _stampsClient = stampsClientService;
            _ratesService = ratesService;
        }

        public async Task<ShipmentLabel> CreateShipmentAsync(Shipping.Abstractions.Models.Shipment shipment, ShipmentRequestDetails shipmentDetails, CancellationToken cancellationToken)
        {
            var ratesDetails = new RateRequestDetails()
            {
                ServiceType = shipmentDetails.SelectedRate.Name.ToUpper() switch
                {

                    "USFC" => StampsServiceType.USPS_FIRST_CLASS_MAIL,
                    "USMM" => StampsServiceType.USPS_MEDIA_MAIL,
                    "USPM" => StampsServiceType.USPS_PRIORITY_MAIL,
                    "USXM" => StampsServiceType.USPS_PRIORITY_MAIL_EXPRESS,
                    "UEMI" => StampsServiceType.USPS_PRIORITY_MAIL_EXPRESS_INTERNATIONAL,
                    "USPMI" => StampsServiceType.USPS_PRIORITY_MAIL_INTERNATIONAL,
                    "USFCI" => StampsServiceType.USPS_FIRST_CLASS_MAIL_INTERNATIONAL,
                    "USPS" => StampsServiceType.USPS_PARCEL_SELECT_GROUND,
                    "USLM" => StampsServiceType.USPS_LIBRARY_MAIL,
                    _ => StampsServiceType.USPS_FIRST_CLASS_MAIL

                },
                ServiceDescription = shipmentDetails.SelectedRate.ServiceName,
                PackageType = shipmentDetails.PackageType,
                DeclaredValue = shipmentDetails.DeclaredValue
            };

            var request = new CreateIndiciumRequest()
            {
                IntegratorTxID = Guid.NewGuid().ToString(),

                CustomerID = Guid.NewGuid().ToString(),

                Customs = shipment.Commodities.Any() ? SetCustomsInformation(shipment, shipmentDetails, ratesDetails) : default,

                SampleOnly = shipmentDetails.IsSample,

                PostageMode = shipmentDetails.LabelOptions.PostageMode.Name switch
                {
                    "NORMAL" => StampsClient.v111.PostageMode.Normal,
                    "NO_POSTAGE" => StampsClient.v111.PostageMode.NoPostage,
                    _ => StampsClient.v111.PostageMode.Normal
                },

                ImageType = shipmentDetails.LabelOptions.ImageType.Name switch
                {
                    "PNG" => StampsClient.v111.ImageType.Png,
                    "PDF" => StampsClient.v111.ImageType.Pdf,
                    _ => StampsClient.v111.ImageType.Png
                },

                EltronPrinterDPIType = shipmentDetails.LabelOptions.DpiType.Name switch
                {
                    "DEFAULT" => EltronPrinterDPIType.Default,
                    "HIGH" => EltronPrinterDPIType.High,
                    _ => EltronPrinterDPIType.Default
                },
                memo = shipmentDetails.LabelOptions.Memo,

                // ??
                cost_code_id = 0,

                deliveryNotification = shipmentDetails.NotificationOptions.IsActive,

                ShipmentNotification = SetShipmentNotification(shipment, shipmentDetails),

                rotationDegrees = shipmentDetails.LabelOptions.RotationDegrees,

                horizontalOffset = shipmentDetails.LabelOptions.HorizontalOffet,

                verticalOffset = shipmentDetails.LabelOptions.VerticalOffet,

                printDensity = shipmentDetails.LabelOptions.PrintDensity,

                printMemo = string.IsNullOrEmpty(shipmentDetails.LabelOptions.Memo) ? false : true,

                printInstructions = shipmentDetails.LabelOptions.PrintInstructions,

                // ??
                requestPostageHash = false,

                nonDeliveryOption = NonDeliveryOption.Undefined,

                // If nonDeliveryOption is Redirect, this address is printed on the CP72 customs form; otherwise, this address is ignored.
                RedirectTo = null,

                // ?
                OutboundTransactionID = string.Empty,

                // ?
                OriginalPostageHash = string.Empty,

                ReturnImageData = shipmentDetails.LabelOptions.ReturnImageData,

                /* For International mail, shipments valued over $2500 generally require an
                    Internal Transaction Number (ITN) to be put on the CP72 form.*/
                InternalTransactionNumber = "123",

                PaperSize = shipmentDetails.LabelOptions.PaperSize.Name switch
                {
                    "DEFAULT" => PaperSizeV1.Default,
                    "LABELSIZE" => PaperSizeV1.LabelSize,
                    "LETTER85X11" => PaperSizeV1.Letter85x11,
                    _ => PaperSizeV1.Default
                },

                EmailLabelTo = shipmentDetails.LabelOptions.EmailLabelTo.IsActivated ? null : SetEmailLabelTo(shipmentDetails),

                // ??
                PayOnPrint = false,

                // ??
                ReturnLabelExpirationDays = 1,

                ImageDpi = shipmentDetails.LabelOptions.ImageDPI.Name switch
                {
                    "DPI203" => ImageDpi.ImageDpi203,
                    "DPI300" => ImageDpi.ImageDpi300,
                    "DPI200" => ImageDpi.ImageDpi200,
                    "DPI150" => ImageDpi.ImageDpi150,
                    "DPI96" => ImageDpi.ImageDpi96,
                    _ => ImageDpi.ImageDpiDefault
                },

                // ??
                RateToken = string.Empty,

                // Caller defined data. Order ID associated to this print.
                OrderId = shipmentDetails.OrderId,

                // ?
                BypassCleanseAddress = false,

                // Identifies the image or logo that will be added to the shipping label.
                ImageId = 0,

                // Caller defined data. 1 - 4
                Reference1 = string.Empty,

                Reference2 = string.Empty,

                Reference3 = string.Empty,

                Reference4 = string.Empty,

                // ??
                ReturnIndiciumData = false,

                // ??
                // ExtendedPostageInfo = new ExtendedPostageInfoV1(),

                // ??
                EnclosedServiceType = EnclosedServiceType.Unknown,

                // ??
                EnclosedPackageType = EnclosedPackageType.Unknown,

                // Caller defined data. Specifies the branding to be used for emails corresponding with this print.
                BrandingId = Guid.NewGuid()
            };

            request = SetOrderDetails(request, shipment);

            try
            {
                var client = _stampsClient.CreateClient();

                var rates = await _ratesService.GetRatesResponseAsync(shipment, ratesDetails, cancellationToken);

                request.Item = await _stampsClient.GetTokenAsync(cancellationToken);

                request.Rate = rates.FirstOrDefault();

                request.ReturnTo = rates.FirstOrDefault()?.From;

                var response = await client.CreateIndiciumAsync(request);

                return new ShipmentLabel()
                {
                    Labels = new List<PackageLabelDetails>()
                    {
                        new PackageLabelDetails()
                        {
                            ProviderLabelId = response.StampsTxID.ToString(),
                            ImageType = shipmentDetails.LabelOptions.ImageType.Name,
                            TrackingId = response.TrackingNumber.ToString(),
                            Bytes = response.ImageData.ToList(),
                            Charges = new PackageCharges()
                            {
                                Surcharges = SetSurcharges(response),
                                TotalSurCharges = SetSurcharges(response).Values.Sum(),
                                NetCharge = response.Rate.Amount
                            }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                var shipmentLabel = new ShipmentLabel();

                shipmentLabel.InternalErrors.Add(ex.Message);

                return shipmentLabel;
            }
        }

        public async Task<CancelIndiciumResponse> CancelShipmentAsync(ShipmentLabel shipmentLabel, CancellationToken cancellationToken)
        {
            var client = _stampsClient.CreateClient();

            var request = new CancelIndiciumRequest()
            {
                Item1 = shipmentLabel.Labels[0].ProviderLabelId
            };
            try
            {
                request.Item = await _stampsClient.GetTokenAsync(cancellationToken);

                return await client.CancelIndiciumAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private Dictionary<string, decimal> SetSurcharges(CreateIndiciumResponse response)
        {
            var surcharges = new Dictionary<string, decimal>();

            if (response.Rate.Surcharges == null)
            {
                return surcharges;
            }

            foreach (var surcharge in response.Rate.Surcharges)
            {
                surcharges[surcharge.SurchargeType.ToString()] = surcharge.Amount;
            }

            return surcharges;
        }

        private CustomsV7 SetCustomsInformation(Shipping.Abstractions.Models.Shipment shipment, ShipmentRequestDetails shipmentDetails, RateRequestDetails rateDetails)
        {
            var customsLine = new List<CustomsLine>();

            foreach (var commodity in shipment.Commodities)
            {
                customsLine.Add(new CustomsLine
                {
                    Description = commodity.Description,
                    CountryOfOrigin = commodity.CountryOfManufacturer,
                    Quantity = commodity.Quantity,
                    HSTariffNumber = string.Empty,
                    sku = commodity.PartNumber,
                    Value = commodity.CustomsValue,
                    WeightLb = (double)commodity.Weight,
                    WeightOz = 0.0d
                });
            }

            return new CustomsV7()
            {
                ContentType = rateDetails.ContentType.Name switch
                {
                    "COMMERCIAL_SAMPLE" => ContentTypeV2.CommercialSample,
                    "DANGEROUS_GOODS" => ContentTypeV2.DangerousGoods,
                    "DOCUMENT" => ContentTypeV2.Document,
                    "GIFT" => ContentTypeV2.Gift,
                    "HUMANITARIAN" => ContentTypeV2.HumanitarianDonation,
                    "MERCHANDISE" => ContentTypeV2.Merchandise,
                    "RETURNED_GOODS" => ContentTypeV2.ReturnedGoods,
                    _ => ContentTypeV2.Other
                },

                Comments = shipment?.Commodities?.FirstOrDefault()?.Comments,

                LicenseNumber = shipment?.Commodities?.FirstOrDefault()?.ExportLicenseNumber,

                CertificateNumber = shipmentDetails.CustomsInformation.CertificateNumber,

                InvoiceNumber = shipmentDetails.CustomsInformation.InvoiceNumber,

                OtherDescribe = shipmentDetails.CustomsInformation.OtherDescribe,

                CustomsLines = customsLine.ToArray(),

                CustomsSigner = shipmentDetails.CustomsInformation.CustomsSigner,

                PassportNumber = shipmentDetails.CustomsInformation.PassportNumber,

                PassportIssueDate = shipmentDetails.CustomsInformation.PassportIsseDate,

                PassportExpiryDate = shipmentDetails.CustomsInformation.PassportExpiryDate,

                ImportLicenseNumber = shipmentDetails.CustomsInformation.ImportLicenseNumber,

                SendersCustomsReference = shipmentDetails.CustomsInformation.SendersCustomsReference
            };
        }

        private ShipmentNotification SetShipmentNotification(Shipping.Abstractions.Models.Shipment shipment, ShipmentRequestDetails shipmentDetails)
        {
            return new ShipmentNotification()
            {
                Email = string.IsNullOrEmpty(shipmentDetails.NotificationOptions.Email) ? shipment.RecipientInfo.Email : shipmentDetails.NotificationOptions.Email,
                CCToAccountHolder = shipmentDetails.NotificationOptions.CC_ToAccountHolder,
                UseCompanyNameInFromLine = shipmentDetails.NotificationOptions.UseCompanyNameInFromLine,
                UseCompanyNameInSubject = shipmentDetails.NotificationOptions.UseCompanyNameInSubject,
            };
        }

        private LabelRecipientInfo SetEmailLabelTo(ShipmentRequestDetails details)
        {
            return new LabelRecipientInfo()
            {
                EmailAddress = details.LabelOptions.EmailLabelTo.Email,
                CopyToOriginator = details.LabelOptions.EmailLabelTo.CopyToOriginator,
                Name = details.LabelOptions.EmailLabelTo.Name,
                Note = details.LabelOptions.EmailLabelTo.Note
            };
        }

        private CreateIndiciumRequest SetOrderDetails(CreateIndiciumRequest request, Shipping.Abstractions.Models.Shipment shipment)
        {
            // request.OrderDetails
            return request;
        }
    }
}
