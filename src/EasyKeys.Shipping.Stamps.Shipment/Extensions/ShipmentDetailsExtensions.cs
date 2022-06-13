using EasyKeys.Shipping.Stamps.Shipment.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment.Extensions;

public static class ShipmentDetailsExtensions
{
    public static CreateIndiciumRequest Map(this ShipmentDetails shipmentDetails)
    {
        var request = new CreateIndiciumRequest()
        {
            IntegratorTxID = shipmentDetails.IntegratorTxId,

            CustomerID = shipmentDetails.CustomerId,

            SampleOnly = shipmentDetails.IsSample,

            PostageMode = shipmentDetails.LabelOptions.PostageMode.Value switch
            {
                (int)PostageMode.Normal => PostageMode.Normal,
                (int)PostageMode.NoPostage => PostageMode.NoPostage,
                _ => PostageMode.Normal
            },

            ImageType = shipmentDetails.LabelOptions.ImageType.Value switch
            {
                (int)StampsClient.v111.ImageType.Png => StampsClient.v111.ImageType.Png,
                (int)StampsClient.v111.ImageType.Pdf => StampsClient.v111.ImageType.Pdf,
                _ => StampsClient.v111.ImageType.Png
            },

            EltronPrinterDPIType = shipmentDetails.LabelOptions.DpiType.Value switch
            {
                (int)EltronPrinterDPIType.Default => EltronPrinterDPIType.Default,
                (int)EltronPrinterDPIType.High => EltronPrinterDPIType.High,
                _ => EltronPrinterDPIType.Default
            },

            memo = shipmentDetails.LabelOptions.Memo,

            cost_code_id = 0,

            deliveryNotification = shipmentDetails.NotificationOptions.IsActive,

            rotationDegrees = shipmentDetails.LabelOptions.RotationDegrees,

            horizontalOffset = shipmentDetails.LabelOptions.HorizontalOffet,

            verticalOffset = shipmentDetails.LabelOptions.VerticalOffet,

            printDensity = shipmentDetails.LabelOptions.PrintDensity,

            printMemo = string.IsNullOrEmpty(shipmentDetails.LabelOptions.Memo) ? false : true,

            printInstructions = shipmentDetails.LabelOptions.PrintInstructions,

            requestPostageHash = false,

            nonDeliveryOption = NonDeliveryOption.Undefined,

            // If nonDeliveryOption is Redirect, this address is printed on the CP72 customs form; otherwise, this address is ignored.
            RedirectTo = null,

            OutboundTransactionID = string.Empty,

            OriginalPostageHash = string.Empty,

            ReturnImageData = shipmentDetails.LabelOptions.ReturnImageData,

            /* For International mail, shipments valued over $2500 generally require an
                Internal Transaction Number (ITN) to be put on the CP72 form.*/
            InternalTransactionNumber = "123",

            PaperSize = shipmentDetails.LabelOptions.PaperSize.Value switch
            {
                (int)PaperSizeV1.Default => PaperSizeV1.Default,
                (int)PaperSizeV1.LabelSize => PaperSizeV1.LabelSize,
                (int)PaperSizeV1.Letter85x11 => PaperSizeV1.Letter85x11,
                _ => PaperSizeV1.Default
            },

            EmailLabelTo = shipmentDetails.LabelOptions.EmailLabelTo.IsActivated ? null : SetEmailLabelTo(shipmentDetails),

            PayOnPrint = false,

            ReturnLabelExpirationDays = 1,

            ImageDpi = shipmentDetails.LabelOptions.ImageDPI.Value switch
            {
                (int)ImageDpi.ImageDpi203 => ImageDpi.ImageDpi203,
                (int)ImageDpi.ImageDpi300 => ImageDpi.ImageDpi300,
                (int)ImageDpi.ImageDpi200 => ImageDpi.ImageDpi200,
                (int)ImageDpi.ImageDpi150 => ImageDpi.ImageDpi150,
                (int)ImageDpi.ImageDpi96 => ImageDpi.ImageDpi96,
                _ => ImageDpi.ImageDpiDefault
            },

            RateToken = string.Empty,

            // Caller defined data. Order ID associated to this print.
            OrderId = shipmentDetails.OrderId,

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
            BrandingId = Guid.NewGuid(),
        };

        return request;
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
