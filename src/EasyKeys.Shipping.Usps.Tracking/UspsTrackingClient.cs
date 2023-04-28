using System.Text;
using System.Xml;
using System.Xml.Serialization;

using EasyKeys.Shipping.Usps.Abstractions.Options;
using EasyKeys.Shipping.Usps.Tracking.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyKeys.Shipping.Usps.Tracking
{
    internal class UspsTrackingClient : IUspsTrackingClient
    {
        private readonly UspsOptions _options;
        private readonly HttpClient _httpClient;
        private readonly ILogger<UspsTrackingClient> _logger;

        public UspsTrackingClient(
            IOptions<UspsOptions> options,
            HttpClient httpClient,
            ILogger<UspsTrackingClient> logger)
        {
            _options = options.Value;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<TrackInfo>> GetTrackInfoAsync(List<TrackID> input, CancellationToken cancellationToken)
        {
            // limit is 10 tracking numbers per request
            var requestGuid = Guid.NewGuid().ToString();
            _logger.LogInformation("New request for {packageTotal} package(s). {requestGuid}", input.Count, requestGuid);

            List<TrackInfo> output = new();
            TrackFieldRequest request;
            var index = 0;

            while (index < input.Count)
            {
                request = new TrackFieldRequest
                {
                    USERID = _options?.UserId,
                    Revision = "1",
                    ClientIp = _options?.ClientIp,
                    TrackID = input.Skip(index).Take(10).ToList(),
                    SourceId = _options?.SourceId ?? nameof(UspsTrackingClient)
                };

                index += 10;

                XmlSerializer xsSubmit = new(typeof(TrackFieldRequest));
                var xml = string.Empty;

                using (var sww = new StringWriter())
                {
                    using var writer = XmlWriter.Create(sww);
                    xsSubmit.Serialize(writer, request);
                    xml = sww.ToString();
                }

                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("API", "TrackV2"),
                    new KeyValuePair<string, string>("XML", xml)
                });

                var response = await _httpClient.PostAsync(string.Empty, formData, cancellationToken).ConfigureAwait(false);

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                try
                {
                    XmlSerializer deserializer = new(typeof(TrackResponse));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    var responseJson = (TrackResponse)deserializer.Deserialize(ms)!;

                    // todo: save response data to correct input data
                    foreach (var trackInfo in responseJson.TrackInfo)
                    {
                        if (trackInfo.Error != null)
                        {
                            _logger.LogWarning(
                                "USPS Returned Error: {uspsErrorNumber} {uspsErrorDescription} {requestGuid}",
                                trackInfo.Error.Number,
                                trackInfo.Error.Description,
                                requestGuid);

                            output.Add(trackInfo);
                        }
                        else
                        {
                            output.Add(trackInfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{requestGuid}", requestGuid);
                    throw new UspsTrackingClientException(ex);
                }
            }

            return output;
        }

        public async Task<TrackInfo> GetTrackInfoAsync(string trackingNumber, CancellationToken cancellationToken)
        {
            List<TrackID> list = new() { new TrackID() { ID = trackingNumber } };

            return (await GetTrackInfoAsync(list, cancellationToken)).FirstOrDefault()!;
        }

        public async Task<List<TrackInfo>> GetTrackInfoAsync(List<string> trackingNumbers, CancellationToken cancellationToken)
        {
            List<TrackID> list = new();
            foreach (var id in trackingNumbers)
            {
                list.Add(new TrackID() { ID = id });
            }

            return await GetTrackInfoAsync(list, cancellationToken);
        }
    }
}
