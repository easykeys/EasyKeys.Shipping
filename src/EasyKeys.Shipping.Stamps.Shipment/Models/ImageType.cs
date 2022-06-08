using Ardalis.SmartEnum;

namespace EasyKeys.Shipping.Stamps.Shipment.Models;

public abstract class ImageType : SmartEnum<ImageType>
{
    public static readonly ImageType Pdf = new PdfType();

    public static readonly ImageType Png = new PngType();

    protected ImageType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }

    public string Description { get; }

    private sealed class PdfType : ImageType
    {
        public PdfType() : base(StampsClient.v111.ImageType.Pdf.ToString(), (int)StampsClient.v111.ImageType.Pdf, "Pdf")
        {
        }
    }

    private sealed class PngType : ImageType
    {
        public PngType() : base(StampsClient.v111.ImageType.Png.ToString(), (int)StampsClient.v111.ImageType.Png, "Png")
        {
        }
    }
}
