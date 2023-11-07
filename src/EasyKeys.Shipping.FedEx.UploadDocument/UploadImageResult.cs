namespace EasyKeys.Shipping.FedEx.UploadDocument;

public class UploadImageResult
{
    public UploadImageResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; set; }

    public string Message { get; set; }
}
