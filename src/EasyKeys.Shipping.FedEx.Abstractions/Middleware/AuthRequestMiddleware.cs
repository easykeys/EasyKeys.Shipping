using System.IO.Compression;
using System.Net;
using System.Text;
using EasyKeys.Shipping.FedEx.Abstractions.Services;

using Microsoft.Extensions.Logging;

namespace EasyKeys.Shipping.FedEx.Abstractions.Middleware;

/// <summary>
/// Middleware class that intercepts fedex api HTTP requests and performs authentication before forwarding them to the server.
/// </summary>
public class AuthRequestMiddleware : DelegatingHandler
{
    private readonly IFedexApiAuthenticatorService _authClient;
    private ILogger<AuthRequestMiddleware> _logger;

    public AuthRequestMiddleware(
        IFedexApiAuthenticatorService authClient,
        ILogger<AuthRequestMiddleware> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _authClient = authClient ?? throw new ArgumentNullException(nameof(authClient));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage();
        try
        {
            // Perform any custom logic here before the request is sent
            _logger.LogDebug($"Intercepted request to {request.RequestUri}");

            var token = await _authClient.GetTokenAsync(cancellationToken);

            request.Headers.Add("Authorization", token);

            // Call the base SendAsync method to continue processing the request
            response = await base.SendAsync(request, cancellationToken);

            if (response.Content.Headers.ContentEncoding.Contains("gzip") && response.IsSuccessStatusCode is not true)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                using var decompressionStream = new GZipStream(responseStream, CompressionMode.Decompress);
                using var decompressedStream = new StreamReader(decompressionStream);
                var decompressedString = await decompressedStream.ReadToEndAsync();
                throw new Exception(decompressedString);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return new HttpResponseMessage
            {
                Content = new StringContent(ex.Message, Encoding.UTF8, "application/json"),
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        return response;
    }
}
