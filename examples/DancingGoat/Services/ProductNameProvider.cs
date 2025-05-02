using System.Collections.Generic;

using DancingGoat.Commerce;
using DancingGoat.Models;

namespace DancingGoat.Services;

public sealed class ProductNameProvider
{
    private readonly IProductVariantsExtractor productVariantsExtractor;


    public ProductNameProvider(IProductVariantsExtractor productVariantsExtractor)
    {
        this.productVariantsExtractor = productVariantsExtractor;
    }


    public string GetProductName(IProductFields product, int? variantId = null)
    {
        var variantValues = product == null ? null : productVariantsExtractor.ExtractVariantsValue(product);
        return FormatProductName(product?.ProductFieldName, variantValues, variantId);
    }


    private static string FormatProductName(string productName, IDictionary<int, string> variants, int? variantId)
    {
        return variants != null && variantId != null && variants.TryGetValue(variantId.Value, out var variantValue)
            ? $"{productName} - {variantValue}"
            : productName;
    }
}
