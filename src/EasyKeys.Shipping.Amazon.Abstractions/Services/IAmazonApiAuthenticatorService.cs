using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyKeys.Shipping.Amazon.Abstractions.Services;

public interface IAmazonApiAuthenticatorService
{
    Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
}

public record AmazonToken(string access_token, string refresh_token,  string token_type, int expires_in);
