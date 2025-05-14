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

public class AmazonToken
{
#pragma warning disable SA1300 // Element should begin with upper-case letter
    public string access_token { get; set; }

    public string refresh_token { get; set; }

    public string token_type { get; set; }

    public int expires_in { get; set; }

#pragma warning restore SA1300 // Element should begin with upper-case letter

    public AmazonToken(string access_token, string refresh_token, string token_type, int expires_in)
    {
        this.access_token = access_token;
        this.refresh_token = refresh_token;
        this.token_type = token_type;
        this.expires_in = expires_in;
    }
}
