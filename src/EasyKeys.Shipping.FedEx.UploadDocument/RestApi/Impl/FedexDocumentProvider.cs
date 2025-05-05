using System.Net.Http.Headers;
using EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.TradeDocumentsUpload;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;
using EasyKeys.Shipping.FedEx.UploadDocument.Models;

using Microsoft.Extensions.Logging;

namespace EasyKeys.Shipping.FedEx.UploadDocument.RestApi.Impl
{
    public class FedexDocumentProvider : IFedExDocumentsProvider
    {
        private readonly ILogger<FedexDocumentProvider> _logger;
        private readonly TradeDocumentsApi _tradeDocumentsApi;
        private readonly IFedexApiAuthenticatorService _authService;

        public FedexDocumentProvider(
            ILogger<FedexDocumentProvider> logger,
            IFedexApiAuthenticatorService authService,
            TradeDocumentsApi tradeDocumentsApi,
            FedExApiOptions options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _tradeDocumentsApi = tradeDocumentsApi ?? throw new ArgumentNullException(nameof(tradeDocumentsApi));
        }

        public async Task<UploadImageResult> UploadImageAsync(JsonPayload payload, ImageAttachment imageAttachment, CancellationToken cancellationToken)
        {
            var result = new UploadImageResult(true, string.Empty);

            try
            {
                var token = await _authService.GetTokenAsync(cancellationToken);

                var jsonString = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new MultipartFormDataContent();
                var jsonContent = new StringContent(jsonString);
                content.Add(jsonContent, payload.FieldName);

                var fileContent = new ByteArrayContent(imageAttachment.Data);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(imageAttachment.ContentType);
                content.Add(fileContent, imageAttachment.FieldName, imageAttachment.Name);

                var t = await _tradeDocumentsApi.Image_Upload_Service_InfoAsync(
                    token,
                    Guid.NewGuid().ToString(),
                    content,
                    cancellationToken);
                result.Message = t.Output.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                result = new UploadImageResult(false, ex.Message);
            }

            return result;
        }
    }
}
