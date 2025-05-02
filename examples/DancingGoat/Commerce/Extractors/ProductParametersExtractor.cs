using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CMS.ContentEngine;

using DancingGoat.Models;

namespace DancingGoat.Commerce
{
    /// <inheritdoc cref="IProductParametersExtractor"/>
    internal class ProductParametersExtractor : IProductParametersExtractor
    {
        private readonly ICollection<IProductTypeParametersExtractor> parametersExtractors;


        public ProductParametersExtractor(ITaxonomyRetriever taxonomyRetriever)
        {
            parametersExtractors =
            [
                new ProductManufacturerExtractor(taxonomyRetriever),
                new CoffeeParametersExtractor(taxonomyRetriever),
                new GrinderParametersExtractor(),
                new ProductTemplateAlphaSizeParametersExtractor(),
            ];
        }

        /// <inheritdoc/>
        public async Task<IDictionary<string, string>> ExtractParameters(IProductFields product, string languageName, CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, string>();

            foreach (var item in parametersExtractors)
            {
                await item.ExtractParameter(parameters, product, languageName, cancellationToken);
            }

            return parameters;
        }
    }
}
