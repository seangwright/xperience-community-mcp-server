using System.Linq;

using DancingGoat.Helpers;

namespace DancingGoat.Models
{
    public record ProductListItemViewModel(string Name, string ImagePath, string Url, decimal Price, string Tag)
    {
        public static ProductListItemViewModel GetViewModel(IProductFields product, string urlPath, string languageName, string tag)
        {
            return new ProductListItemViewModel(
                            product.ProductFieldName,
                            product.ProductFieldImage.FirstOrDefault()?.ImageFile.Url,
                            urlPath,
                            product.ProductFieldPrice,
                            tag);
        }
    }
}
