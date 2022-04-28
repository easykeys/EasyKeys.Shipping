namespace EasyKeys.Shipping.Stamps.Abstractions.Models;

public enum PackageType
{
    /// <summary>
    /// Unknown. Do not use.
    /// </summary>
    Unknown,

    /// <summary>
    /// Postcard. Enabled on request.
    /// </summary>
    PostCard,

    /// <summary>
    /// Letter. Enabled on request.
    /// </summary>
    Letter,

    /// <summary>
    /// Large envelope or flat. Has one dimension that is between 11 ½” and 15” long, 6 1/8” and 12” high, or ¼” and ¾ thick.
    /// </summary>
    Large_Envelope_Or_Flat,

    /// <summary>
    /// Thick envelope. Envelopes or flats greater than ¾” at the thickest point.
    /// </summary>
    Thick_Envelope,

    /// <summary>
    /// Package. Longest side plus the distance around the thickest part is less than or equal to 84”
    /// </summary>
    Package,

    /// <summary>
    /// USPS small flat rate box. A special 8-5/8” x 5-3/8” x 1-5/8” USPS box that clearly indicates “Small Flat Rate Box”.
    /// </summary>
    Small_Flat_Rate_Box,

    /// <summary>
    /// USPS medium flat rate box. A special 11” x 8 ½” x 5 ½” or 14” x 3.5” x 12” USPS box that clearly indicates “Medium Flat Rate Box”
    /// </summary>
    Flat_Rate_Box,

    /// <summary>
    /// USPS large flat rate box. A special 12” x 12” x 6” USPS box that clearly indicates “Large Flat Rate Box”.
    /// </summary>
    Large_Flat_Rate_Box,

    /// <summary>
    /// USPS flat rate envelope. A special cardboard envelope provided by the USPS that clearly indicates “Flat Rate”.
    /// </summary>
    Flat_Rate_Envelope,

    /// <summary>
    /// USPS flat rate padded envelope.
    /// </summary>
    Flat_Rate_Padded_Envelope,

    /// <summary>
    /// Large package. Longest side plus the distance around the thickest part is over 84” and less than or equal to 108”.
    /// </summary>
    Large_Package,

    /// <summary>
    /// Oversized package. Longest side plus the distance around the thickest part is over 108” and less than or equal to 130”.
    /// </summary>
    Oversized_Package,

    /// <summary>
    /// USPS regional rate box A. A special 10 15/16” x 2 3/8” x 12 13/ 16” or 10” x 7” x 4 3/4” USPS box that clearly indicates “Regional Rate Box A”. 15 lbs maximum weight.
    /// </summary>
    Regional_Rate_Box_A,

    /// <summary>
    /// USPS regional rate box B. A special 14 3/8” x 2 2/8” x 15 7/8” or 12” x 10 1/4” x 5” USPS box that clearly indicates “Regional Rate Box B”. 20 lbs maximum weight.
    /// </summary>
    Regional_Rate_Box_B,

    /// <summary>
    /// USPS regional rate box C. A special 15” x 12” x 12” USPS box that clearly indicates ”Regional Rate Box C”. 25 lbs maximum weight.
    /// </summary>
    Regional_Rate_Box_C,

    /// <summary>
    /// USPS-supplied Priority Mail flat-rate envelope 9 1/2” x 15”. Maximum weight 4 pounds.
    /// </summary>
    Legal_Flat_Rate_Envelope,

    /// <summary>
    /// This feature is not available for all integrations.
    /// </summary>
    Express_Envelope,

    /// <summary>
    /// This feature is not available for all integrations.
    /// </summary>
    Documents,

    /// <summary>
    /// This feature is not available for all integrations.
    /// </summary>
    Envelope,

    /// <summary>
    /// This feature is not available for all integrations.
    /// </summary>
    Pak
}
