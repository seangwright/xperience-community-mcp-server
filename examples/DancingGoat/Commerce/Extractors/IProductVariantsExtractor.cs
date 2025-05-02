using System.Collections.Generic;

using DancingGoat.Models;

namespace DancingGoat.Commerce
{
    /// <summary>
    /// Extractor of product-specific variants.
    /// </summary>
    public interface IProductVariantsExtractor
    {
        /// <summary>
        /// Extract product variants and update the dictionary of variants.
        /// </summary>
        /// <param name="product">Product to process.</param>
        /// <returns>Dictionary containing product variants.</returns>
        IDictionary<int, string> ExtractVariantsValue(IProductFields product);


        /// <summary>
        /// Extract product variants SKU code and update the dictionary of variants.
        /// </summary>
        /// <param name="product">Product to process.</param>
        /// <returns>Dictionary containing product variants SKU code.</returns>
        IDictionary<int, string> ExtractVariantsSKUCode(IProductFields product);
    }
}
