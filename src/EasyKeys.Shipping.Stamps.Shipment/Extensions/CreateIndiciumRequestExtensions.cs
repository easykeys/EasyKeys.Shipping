using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates.Extensions;
using EasyKeys.Shipping.Stamps.Rates.Models;
using EasyKeys.Shipping.Stamps.Shipment.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment.Extensions;

public static class CreateIndiciumRequestExtensions
{
    public static CreateIndiciumRequest MapToShipmentRequest(
        this CreateIndiciumRequest request,
        bool isDomestic,
        double weightLb,
        ShipmentDetails shipmentDetails,
        RateOptions rateOptions)
    {
        request.IntegratorTxID = shipmentDetails.IntegratorTxId;

        request.CustomerID = shipmentDetails.CustomerId;

        request.SampleOnly = shipmentDetails.IsSample;

        request.PostageMode = shipmentDetails.LabelOptions.PostageMode.Value switch
        {
            (int)PostageMode.Normal => PostageMode.Normal,
            (int)PostageMode.NoPostage => PostageMode.NoPostage,
            _ => PostageMode.Normal
        };

        request.ImageType = shipmentDetails.LabelOptions.ImageType.Value switch
        {
            (int)StampsClient.v111.ImageType.Png => StampsClient.v111.ImageType.Png,
            (int)StampsClient.v111.ImageType.Pdf => StampsClient.v111.ImageType.Pdf,
            _ => StampsClient.v111.ImageType.Png
        };

        request.EltronPrinterDPIType = shipmentDetails.LabelOptions.DpiType.Value switch
        {
            (int)EltronPrinterDPIType.Default => EltronPrinterDPIType.Default,
            (int)EltronPrinterDPIType.High => EltronPrinterDPIType.High,
            _ => EltronPrinterDPIType.Default
        };

        request.memo = shipmentDetails.LabelOptions.Memo;

        request.cost_code_id = 0;

        request.deliveryNotification = shipmentDetails.NotificationOptions.IsActive;

        request.rotationDegrees = shipmentDetails.LabelOptions.RotationDegrees;

        request.horizontalOffset = shipmentDetails.LabelOptions.HorizontalOffet;

        request.verticalOffset = shipmentDetails.LabelOptions.VerticalOffet;

        request.printDensity = shipmentDetails.LabelOptions.PrintDensity;

        request.printMemo = string.IsNullOrEmpty(shipmentDetails.LabelOptions.Memo) ? false : true;

        request.printInstructions = shipmentDetails.LabelOptions.PrintInstructions;

        request.requestPostageHash = false;

        request.nonDeliveryOption = NonDeliveryOption.Undefined;

        // If nonDeliveryOption is Redirect; this address is printed on the CP72 customs form; otherwise; this address is ignored.
        request.RedirectTo = null;

        request.OutboundTransactionID = string.Empty;

        request.OriginalPostageHash = string.Empty;

        request.ReturnImageData = shipmentDetails.LabelOptions.ReturnImageData;

        /* For International mail; shipments valued over $2500 generally require an
            Internal Transaction Number (ITN) to be put on the CP72 form.*/
        request.InternalTransactionNumber = "123";

        request.PaperSize = shipmentDetails.LabelOptions.PaperSize.Value switch
        {
            (int)PaperSizeV1.Default => PaperSizeV1.Default,
            (int)PaperSizeV1.LabelSize => PaperSizeV1.LabelSize,
            (int)PaperSizeV1.Letter85x11 => PaperSizeV1.Letter85x11,
            _ => PaperSizeV1.Default
        };

        request.EmailLabelTo = shipmentDetails.LabelOptions.EmailLabelTo.IsActivated ? null : SetEmailLabelTo(shipmentDetails);

        request.PayOnPrint = false;

        request.ReturnLabelExpirationDays = 1;

        request.ImageDpi = shipmentDetails.LabelOptions.ImageDPI.Value switch
        {
            (int)ImageDpi.ImageDpi203 => ImageDpi.ImageDpi203,
            (int)ImageDpi.ImageDpi300 => ImageDpi.ImageDpi300,
            (int)ImageDpi.ImageDpi200 => ImageDpi.ImageDpi200,
            (int)ImageDpi.ImageDpi150 => ImageDpi.ImageDpi150,
            (int)ImageDpi.ImageDpi96 => ImageDpi.ImageDpi96,
            _ => ImageDpi.ImageDpiDefault
        };

        request.RateToken = string.Empty;

        // Caller defined data. Order ID associated to this print.
        request.OrderId = shipmentDetails.OrderId;

        request.BypassCleanseAddress = false;

        // Identifies the image or logo that will be added to the shipping label.
        request.ImageId = 0;

        // Caller defined data. 1 - 4
        request.Reference1 = string.Empty;

        request.Reference2 = string.Empty;

        request.Reference3 = string.Empty;

        request.Reference4 = string.Empty;

        // ??
        request.ReturnIndiciumData = false;

        // ??
        // ExtendedPostageInfo = new ExtendedPostageInfoV1();

        // ??
        request.EnclosedServiceType = EnclosedServiceType.Unknown;

        // ??
        request.EnclosedPackageType = EnclosedPackageType.Unknown;

        // Caller defined data. Specifies the branding to be used for emails corresponding with this print.
        request.BrandingId = Guid.NewGuid();

        SetShipmentNotification(
            request,
            shipmentDetails,
            rateOptions);

        if (!isDomestic)
        {
            SetCustomsInformation(
                request,
                rateOptions.ContentType,
                shipmentDetails,
                weightLb);
        }

        return request;
    }

    private static CreateIndiciumRequest SetShipmentNotification(
    this CreateIndiciumRequest request,
    ShipmentDetails shipmentDetails,
    RateOptions rateOptions)
    {
        request.ShipmentNotification = new ShipmentNotification()
        {
            Email = string.IsNullOrEmpty(shipmentDetails.NotificationOptions.Email) ? rateOptions.Recipient.Email : shipmentDetails.NotificationOptions.Email,
            CCToAccountHolder = shipmentDetails.NotificationOptions.CC_ToAccountHolder,
            UseCompanyNameInFromLine = shipmentDetails.NotificationOptions.UseCompanyNameInFromLine,
            UseCompanyNameInSubject = shipmentDetails.NotificationOptions.UseCompanyNameInSubject,
        };

        return request;
    }

    private static void SetCustomsInformation(
    this CreateIndiciumRequest request,
    StampsContentType contentType,
    ShipmentDetails shipmentDetails,
    double weightLb)
    {
        var customsLine = new List<CustomsLine>();

        foreach (var commodity in shipmentDetails.Commodities)
        {
            customsLine.Add(new CustomsLine
            {
                Description = commodity.Description,
                CountryOfOrigin = commodity.CountryOfManufacturer,
                Quantity = commodity.Quantity,
                HSTariffNumber = string.Empty,
                sku = commodity.PartNumber,
                Value = commodity.CustomsValue,
                WeightLb = weightLb,
                WeightOz = 0.0d
            });
        }

        request.Customs = new CustomsV7()
        {
            ContentType = contentType.Map(),

            Comments = shipmentDetails.Commodities?.FirstOrDefault()?.Comments,

            LicenseNumber = shipmentDetails.Commodities?.FirstOrDefault()?.ExportLicenseNumber,

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

    private static LabelRecipientInfo SetEmailLabelTo(ShipmentDetails details)
    {
        return new LabelRecipientInfo()
        {
            EmailAddress = details.LabelOptions.EmailLabelTo.Email,
            CopyToOriginator = details.LabelOptions.EmailLabelTo.CopyToOriginator,
            Name = details.LabelOptions.EmailLabelTo.Name,
            Note = details.LabelOptions.EmailLabelTo.Note
        };
    }
}
