using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EasyKeys.Shipping.FedEx.Abstractions.Api.V1.Auth;

public interface IFedExAuthClient
{
    Task<string> GetTokenAsync();
}
