using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.TradeDocumentsUpload;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;

using Microsoft.Extensions.Logging;

using static System.Net.Mime.MediaTypeNames;

namespace EasyKeys.Shipping.FedEx.UploadDocument.RestApi.Impl
{
    public class FedexDocumentProvider : IFedExDocumentsProvider
    {
        private readonly ILogger<FedexDocumentProvider> _logger;
        private readonly TradeDocumentsApi _tradeDocumentsApi;
        private readonly FedExApiOptions _options;
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
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<UploadImageResult> UploadImageAsync(byte[] image, int imageId, CancellationToken cancellationToken)
        {
            var result = new UploadImageResult(true, string.Empty);

            try
            {
                var token = await _authService.GetTokenAsync(cancellationToken);

                // Create multipart content
                using var multipartContent = new MultipartFormDataContent();

                // JSON payload as stream
                var documentObject = new
                {
                    document = new
                    {
                        referenceId = "1234",
                        name = "Images/ek_logo_main4 (Custom).gif",
                        contentType = "image/png",
                        meta = new
                        {
                            imageType = "SIGNATURE",
                            imageIndex = "IMAGE_1"
                        }
                    },
                    rules = new
                    {
                        workflowName = "LetterheadSignature"
                    }
                };

                var json = JsonSerializer.Serialize(documentObject);
                var jsonBytes = Encoding.UTF8.GetBytes(json);
                var jsonStream = new MemoryStream(jsonBytes);
                var jsonContent = new StreamContent(jsonStream);
                jsonContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                multipartContent.Add(jsonContent, "document");

                // Add file stream
                var fileContent = new StreamContent(new MemoryStream(image));

                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                multipartContent.Add(fileContent, "attachment", "Images/ek_logo_main4 (Custom).gif");

                var t = await _tradeDocumentsApi.Image_Upload_Service_InfoAsync(
                    token,
                    Guid.NewGuid().ToString(),
                    multipartContent.ReadAsStream(),
                    cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                result = new UploadImageResult(false, ex.Message);
            }

            return result;
        }
    }
}
