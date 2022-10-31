namespace EasyKeys.Shipping.Abstractions.Models;

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
public struct Dimensions
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
{
    public Dimensions(
        decimal length,
        decimal width,
        decimal height)
    {
        Length = length;
        Width = width;
        Height = height;
    }

    public decimal Length { get; set; }

    public decimal RoundedLength => Math.Ceiling(Length);

    public decimal Width { get; set; }

    public decimal RoundedWidth => Math.Ceiling(Width);

    public decimal Height { get; set; }

    public decimal RoundedHeight => Math.Ceiling(Height);

    /// <summary>
    /// Package measurement is (w*2) + (h*2) + l.
    /// This is used to determine if the package is oversize or not.
    /// </summary>
    public decimal Measurement
    {
        get
        {
            var result = (Width * 2) + (Height * 2) + Length;
            return Math.Ceiling(result);
        }
    }

    public decimal Girth
    {
        get
        {
            var result = (Width * 2) + (Height * 2);
            return Math.Ceiling(result);
        }
    }

    public static bool operator ==(Dimensions dim1, Dimensions dim2) => dim1.Equals(dim2);

    public static bool operator !=(Dimensions dim1, Dimensions dim2) => !dim1.Equals(dim2);
}
