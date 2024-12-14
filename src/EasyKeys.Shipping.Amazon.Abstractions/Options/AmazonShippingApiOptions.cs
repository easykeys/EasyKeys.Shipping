using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyKeys.Shipping.Amazon.Abstractions.Options;
public class AmazonShippingApiOptions
{
    required public string ClientId { get; set; }

    required public string ClientSecret { get; set; }
    
    required public string RefreshToken { get; set; } 
}
