using DancingGoat.Models;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DancingGoat.Commerce
{
    /// <summary>
    /// Extractor of product-specific parameters.
    /// </summary>
    public interface IProductParametersExtractor
    {
        /// <summary>
        /// Extract product parameters and update the dictionary of parameters.
        /// </summary>
        /// <param name="product">Product to process.</param>
        /// <param name="languageName">Language name used.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Dictionary containing product parameters.</returns>
        Task<IDictionary<string, string>> ExtractParameters(IProductFields product, string languageName, CancellationToken cancellationToken);
    }
}
