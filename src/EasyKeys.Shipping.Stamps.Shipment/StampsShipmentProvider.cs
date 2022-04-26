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
            var client = _stampsClient.CreateClient();

            var ratesDetails = new RateRequestDetails()
            {
                ServiceType = shipmentDetails.SelectedRate.Name switch
                {
                    "USFC" => Abstractions.Models.ServiceType.USPS_FIRST_CLASS_MAIL,
                    "USMM" => Abstractions.Models.ServiceType.USPS_MEDIA_MAIL,
                    "USPM" => Abstractions.Models.ServiceType.USPS_PRIORITY_MAIL,
                    "USXM" => Abstractions.Models.ServiceType.USPS_PRIORITY_MAIL_EXPRESS,
                    "UEMI" => Abstractions.Models.ServiceType.USPS_PRIORITY_MAIL_EXPRESS_INTERNATIONAL,
                    "UPMI" => Abstractions.Models.ServiceType.USPS_PRIORITY_MAIL_INTERNATIONAL,
                    "USFCI" => Abstractions.Models.ServiceType.USPS_FIRST_CLASS_MAIL_INTERNATIONAL,
                    "USPS" => Abstractions.Models.ServiceType.USPS_PARCEL_SELECT_GROUND,
                    "USLM" => Abstractions.Models.ServiceType.USPS_LIBRARY_MAIL,
                    _ => Abstractions.Models.ServiceType.USPS_PRIORITY_MAIL

                },
                ServiceDescription = shipmentDetails.SelectedRate.ServiceName,
            };

            var rates = await _ratesService.GetRatesResponseAsync(shipment, ratesDetails, cancellationToken);

            var request = new CreateIndiciumRequest()
            {
                Item = await _stampsClient.GetTokenAsync(cancellationToken),

                IntegratorTxID = Guid.NewGuid().ToString(),

                Rate = rates.FirstOrDefault(),

                ReturnTo = rates?.FirstOrDefault()?.From,

                CustomerID = Guid.NewGuid().ToString(),

                Customs = shipment.Commodities.Any() ? SetCustomsInformation(shipment, shipmentDetails, ratesDetails) : default,

                SampleOnly = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Development" ? true : false,

                PostageMode = shipmentDetails.LabelOptions.PostageMode.ToLower() switch
                {
                    "normal" => PostageMode.Normal,
                    "NoPostage" => PostageMode.NoPostage,
                    _ => PostageMode.Normal
                },

                ImageType = shipmentDetails.LabelOptions.ImageType switch
                {
                    "png" => ImageType.Png,
                    "pdf" => ImageType.Pdf,
                    _ => ImageType.Png
                },

                EltronPrinterDPIType = shipmentDetails.LabelOptions.Resolution switch
                {
                    "default" => EltronPrinterDPIType.Default,
                    "high" => EltronPrinterDPIType.High,
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
                OutboundTransactionID = String.Empty,

                // ?
                OriginalPostageHash = String.Empty,

                ReturnImageData = shipmentDetails.LabelOptions.ReturnImageData,

                /* For International mail, shipments valued over $2500 generally require an
                    Internal Transaction Number (ITN) to be put on the CP72 form.*/
                InternalTransactionNumber = "123",

                PaperSize = shipmentDetails.LabelOptions.PaperSize.ToLower() switch
                {
                    "default" => PaperSizeV1.Default,
                    "labelsize" => PaperSizeV1.LabelSize,
                    "letter85x11" => PaperSizeV1.Letter85x11,
                    _ => PaperSizeV1.Default
                },

                EmailLabelTo = string.IsNullOrEmpty(shipmentDetails.LabelOptions.EmailLabelTo.Email) ? null : SetEmailLabelTo(shipmentDetails),

                // ??
                PayOnPrint = false,

                // ??
                ReturnLabelExpirationDays = 1,

                ImageDpi = shipmentDetails.LabelOptions.ImageDPI switch
                {
                    "ImageDpi203" => ImageDpi.ImageDpiDefault,
                    "ImageDpi300" => ImageDpi.ImageDpi300,
                    "ImageDpi200" => ImageDpi.ImageDpi200,
                    "ImageDpi150" => ImageDpi.ImageDpi150,
                    "ImageDpi96" => ImageDpi.ImageDpi96,
                    _ => ImageDpi.ImageDpiDefault
                },

                // ??
                RateToken = string.Empty,

                // Caller defined data. Order ID associated to this print.
                OrderId = string.Empty,

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
                //ExtendedPostageInfo = new ExtendedPostageInfoV1(),

                // ??
                EnclosedServiceType = EnclosedServiceType.Unknown,

                // ??
                EnclosedPackageType = EnclosedPackageType.Unknown,

                //Caller defined data. Specifies the branding to be used for emails corresponding with this print.
                BrandingId = Guid.NewGuid(),
            };

            request = SetOrderDetails(request, shipment);

            try
            {
                var response = await client.CreateIndiciumAsync(request);

                return new ShipmentLabel()
                {
                    Labels = new List<PackageLabelDetails>()
                    {
                        new PackageLabelDetails()
                        {
                            ProviderLabelId = response.StampsTxID.ToString(),
                            ImageType = shipmentDetails.LabelOptions.ImageType,
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
                return new ShipmentLabel();
            }
        }

        public async Task<CancelIndiciumResponse> CancelShipmentAsync(ShipmentLabel shipmentLabel, CancellationToken cancellationToken)
        {
            var client = _stampsClient.CreateClient();

            var request = new CancelIndiciumRequest()
            {
                Item = await _stampsClient.GetTokenAsync(cancellationToken),
                Item1 = shipmentLabel.Labels[0].ProviderLabelId
            };
            try
            {
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
                ContentType = rateDetails.ContentType.ToLower() switch
                {
                    "commercial_sample" => ContentTypeV2.CommercialSample,
                    "dangerous_goods" => ContentTypeV2.DangerousGoods,
                    "document" => ContentTypeV2.Document,
                    "gift" => ContentTypeV2.Gift,
                    "humanitarian" => ContentTypeV2.HumanitarianDonation,
                    "merchandise" => ContentTypeV2.Merchandise,
                    "returned_goods" => ContentTypeV2.ReturnedGoods,
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
                Email = string.IsNullOrEmpty(shipmentDetails.NotificationOptions.Email) ? shipment.RecipientInformation.Email : shipmentDetails.NotificationOptions.Email,
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
            //request.OrderDetails
            return request;
        }
    }
}
