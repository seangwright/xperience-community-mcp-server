using CMS.Commerce;

#pragma warning disable KXE0002 // Commerce feature is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

namespace DancingGoat.Commerce;

/// <summary>
/// Represents the Dancing goat price formatter.
/// </summary>
internal sealed class RazorPriceFormatter
{
    private readonly IPriceFormatter priceFormatter;


    public RazorPriceFormatter(IPriceFormatter priceFormatter)
    {
        this.priceFormatter = priceFormatter;
    }


    public string Format(decimal price)
    {
        return priceFormatter.Format(price, new PriceFormatConfiguration());
    }
}
#pragma warning restore KXE0002 // Commerce feature is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
