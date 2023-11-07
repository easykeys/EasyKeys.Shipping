namespace EasyKeys.Shipping.FedEx.UploadDocument
{
    /// <summary>
    /// upload images.
    /// </summary>
    public interface IFedExDocumentsProvider
    {
        /// <summary>
        /// ImageId can be any number 0 -4.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        Task<UploadImageResult> UploadImageAsync(byte[] image, int imageId);
    }
}
