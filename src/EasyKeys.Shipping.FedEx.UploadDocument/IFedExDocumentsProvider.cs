using System.Net;
using System.Runtime.Intrinsics.X86;

using EasyKeys.Shipping.FedEx.UploadDocument.Models;

namespace EasyKeys.Shipping.FedEx.UploadDocument
{
    /// <summary>
    /// upload images.
    /// </summary>
    public interface IFedExDocumentsProvider
    {
        /// <summary>
        ///         Use this endpoint to upload customized Company Letterhead/Logo and Digital signature images which can be used in the FedEx generated paperwork or reports.
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="imageAttachment"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UploadImageResult> UploadImageAsync(JsonPayload payload, ImageAttachment imageAttachment, CancellationToken cancellationToken = default);
    }
}
