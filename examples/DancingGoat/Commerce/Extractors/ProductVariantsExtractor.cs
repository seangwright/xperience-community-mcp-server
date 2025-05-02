using System.Collections.Generic;

using DancingGoat.Models;

namespace DancingGoat.Commerce
{
    /// <inheritdoc cref="IProductVariantsExtractor"/>
    internal class ProductVariantsExtractor : IProductVariantsExtractor
    {
        private readonly ICollection<IProductTypeVariantsExtractor> parametersExtractors;


        public ProductVariantsExtractor()
        {
            parametersExtractors =
            [
                new ProductTemplateAlphaSizeVariantsExtractor()
            ];
        }


        /// <inheritdoc/>
        public IDictionary<int, string> ExtractVariantsValue(IProductFields product)
        {
            var result = new Dictionary<int, string>();

            foreach (var item in parametersExtractors)
            {
                var variants = item.ExtractVariantsValue(product);
                if (variants != null)
                {
                    foreach (var variant in variants)
                    {
                        result.Add(variant.Key, variant.Value);
                    }
                }
            }

            return result;
        }


        /// <inheritdoc/>
        public IDictionary<int, string> ExtractVariantsSKUCode(IProductFields product)
        {
            var result = new Dictionary<int, string>();

            foreach (var item in parametersExtractors)
            {
                var variants = item.ExtractVariantsSKUCode(product);
                if (variants != null)
                {
                    foreach (var variant in variants)
                    {
                        result.Add(variant.Key, variant.Value);
                    }
                }
            }

            return result;
        }
    }
}
