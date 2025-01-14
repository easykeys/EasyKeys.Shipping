using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Amazon.Rates.Models;

public class RateContactInfo
{
    public ContactInfo SenderContact { get; set; } = new ();

    public ContactInfo RecipientContact { get; set; } = new ();
}
