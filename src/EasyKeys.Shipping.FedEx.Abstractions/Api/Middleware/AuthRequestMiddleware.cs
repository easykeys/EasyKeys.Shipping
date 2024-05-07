using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using EasyKeys.Shipping.FedEx.Abstractions.Api.V1.Auth;

using Microsoft.Extensions.Logging;

namespace EasyKeys.Shipping.FedEx.Abstractions.Api.Middleware;
public class AuthRequestMiddleware : DelegatingHandler
{
    private readonly IFedExAuthClient _authClient;
    private ILogger<AuthRequestMiddleware> _logger;

    public AuthRequestMiddleware(
        IFedExAuthClient authClient,
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

            var token = await _authClient.GetTokenAsync();

            request.Headers.Add("Authorization", token);

            // Call the base SendAsync method to continue processing the request
            response = await base.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }

        return response;
    }
}
