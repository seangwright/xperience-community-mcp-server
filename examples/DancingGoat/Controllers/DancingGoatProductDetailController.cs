using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CMS.ContentEngine;

using DancingGoat;
using DancingGoat.Commerce;
using DancingGoat.Controllers;
using DancingGoat.Models;
using DancingGoat.Services;

using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;

using Microsoft.AspNetCore.Mvc;

[assembly: RegisterWebPageRoute(ProductPage.CONTENT_TYPE_NAME, typeof(DancingGoatProductDetailController), WebsiteChannelNames = [DancingGoatConstants.WEBSITE_CHANNEL_NAME])]

namespace DancingGoat.Controllers
{
    public class DancingGoatProductDetailController : Controller
    {
        private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
        private readonly ProductPageRepository productPageRepository;
        private readonly IProductParametersExtractor productParametersExtractor;
        private readonly IProductVariantsExtractor productVariantsExtractor;
        private readonly ITagTitleRetriever tagTitleRetriever;
        private readonly IPreferredLanguageRetriever currentLanguageRetriever;


        public DancingGoatProductDetailController(
            IWebPageDataContextRetriever webPageDataContextRetriever,
            ProductPageRepository productPageRepository,
            IProductParametersExtractor productParametersExtractor,
            IProductVariantsExtractor productVariantsExtractor,
            ITagTitleRetriever tagTitleRetriever,
            IPreferredLanguageRetriever currentLanguageRetriever)
        {
            this.webPageDataContextRetriever = webPageDataContextRetriever;
            this.productPageRepository = productPageRepository;
            this.productParametersExtractor = productParametersExtractor;
            this.productVariantsExtractor = productVariantsExtractor;
            this.tagTitleRetriever = tagTitleRetriever;
            this.currentLanguageRetriever = currentLanguageRetriever;
        }


        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            // Web page identification data
            var webPageData = webPageDataContextRetriever.Retrieve().WebPage;
            var webPageItemId = webPageData.WebPageItemID;
            var languageName = currentLanguageRetriever.Get();

            var productPage = await productPageRepository.GetProductPage(webPageItemId, languageName, cancellationToken);
            if (productPage == null)
            {
                return NotFound();
            }

            var productItem = productPage.ProductPageProduct.FirstOrDefault() as IProductFields;

            var tag = productItem.ProductFieldTags.Any() ? await tagTitleRetriever.GetTagTitle(productItem.ProductFieldTags.First().Identifier, languageName, cancellationToken) : null;

            var parameters = await productParametersExtractor.ExtractParameters(productItem, languageName, cancellationToken);

            var variantValues = productVariantsExtractor.ExtractVariantsValue(productItem);

            int contentItemId = (productItem as IContentItemFieldsSource).SystemFields.ContentItemID;

            return View(new ProductViewModel(productItem.ProductFieldName, productItem.ProductFieldDescription, productItem.ProductFieldImage.FirstOrDefault()?.ImageFile.Url, productItem.ProductFieldPrice, tag, contentItemId, parameters, variantValues));
        }
    }
}
