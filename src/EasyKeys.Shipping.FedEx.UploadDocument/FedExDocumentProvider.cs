using System.Xml.Serialization;

using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using UploadDocumentService_v19;

namespace EasyKeys.Shipping.FedEx.UploadDocument;
public class FedExDocumentProvider : IFedExDocumentsProvider
{
    private readonly ILogger<FedExDocumentProvider> _logger;
    private readonly UploadDocumentPortType _fedExClient;
    private FedExOptions _options;

    public FedExDocumentProvider(
        ILogger<FedExDocumentProvider> logger,
        IFedExClientService fedExClient,
        IOptionsMonitor<FedExOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fedExClient = fedExClient.CreateUploadDocumentClient() ?? throw new ArgumentNullException(nameof(fedExClient));
        _options = options.CurrentValue ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<UploadImageResult> UploadImageAsync(byte[] image, int imageId)
    {
        try
        {
            var request = new uploadImagesRequest1()
            {
                UploadImagesRequest = new UploadImagesRequest()
                {
                    WebAuthenticationDetail = new WebAuthenticationDetail
                    {
                        UserCredential = new WebAuthenticationCredential
                        {
                            Key = _options.FedExKey,
                            Password = _options.FedExPassword
                        }
                    },
                    ClientDetail = new ClientDetail
                    {
                        AccountNumber = _options.FedExAccountNumber,
                        MeterNumber = _options.FedExMeterNumber
                    },
                    TransactionDetail = new TransactionDetail()
                    {
                        CustomerTransactionId = "UploadImagesRequest v19",
                        Localization = new Localization()
                        {
                            LanguageCode = "EN",
                            LocaleCode = "US"
                        }
                    },
                    Version = new VersionId()
                    {
                        ServiceId = "cdus",
                        Major = 19,
                        Intermediate = 0,
                        Minor = 0
                    },
                    Images = new List<UploadImageDetail>
                {
                    new UploadImageDetail()
                    {
                        Id = (ImageId)imageId,
                        IdSpecified = true,
                        Image = image
                    }
                }.ToArray()
                }
            };

            var response = await _fedExClient.uploadImagesAsync(request);

            var reply = response.UploadImagesReply;

            if ((reply?.HighestSeverity == NotificationSeverityType.ERROR)
                        || (reply?.HighestSeverity == NotificationSeverityType.FAILURE))
            {
                return new UploadImageResult(false, reply.Notifications.Select(x => x.Message).Flatten(","));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{providerName} failed", nameof(FedExDocumentProvider));
        }

        return new UploadImageResult(true, string.Empty);
    }
}
