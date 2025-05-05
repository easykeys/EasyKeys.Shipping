namespace EasyKeys.Shipping.FedEx.UploadDocument.Models;

public class ImageAttachment
{
    public byte[] Data { get; set; }

    public string Name { get; set; }             // e.g., "signature.PNG"

    public string FieldName { get; set; } = "attachment"; // form field name

    public string ContentType { get; set; } = "image/png";

    public string ImageType { get; set; }        // e.g., "SIGNATURE"
}
