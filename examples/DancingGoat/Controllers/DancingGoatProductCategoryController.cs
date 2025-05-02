using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CMS.ContentEngine;
using CMS.Websites;

using DancingGoat;
using DancingGoat.Controllers;
using DancingGoat.Models;
using DancingGoat.ViewComponents;

using Kentico.Content.Web.Mvc.Routing;
using Kentico.Content.Web.Mvc;

using Microsoft.AspNetCore.Mvc;

[assembly: RegisterWebPageRoute(ProductCategory.CONTENT_TYPE_NAME, typeof(DancingGoatProductCategoryController), WebsiteChannelNames = [DancingGoatConstants.WEBSITE_CHANNEL_NAME])]

namespace DancingGoat.Controllers
{
    public class DancingGoatProductCategoryController : Controller
    {
        private readonly ProductRepository productRepository;
        private readonly ProductPageRepository productPageRepository;
        private readonly NavigationService navigationService;
        private readonly ProductCategoryRepository productCategoryRepository;
        private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
        private readonly ITaxonomyRetriever taxonomyRetriever;
        private readonly IPreferredLanguageRetriever currentLanguageRetriever;

        private const string PRODUCT_CATEGORY_FIELD_NAME = "ProductFieldCategory";
        private const string PRODUCT_TYPE_SELECTION_OTHER_VALUE = "Other";


        public DancingGoatProductCategoryController(ProductRepository productRepository, ProductCategoryRepository productCategoryRepository, ProductPageRepository productPageRepository,
            NavigationService navigationService, IWebPageDataContextRetriever webPageDataContextRetriever, ITaxonomyRetriever taxonomyRetriever, IPreferredLanguageRetriever currentLanguageRetriever)
        {
            this.webPageDataContextRetriever = webPageDataContextRetriever;
            this.productCategoryRepository = productCategoryRepository;
            this.productRepository = productRepository;
            this.productPageRepository = productPageRepository;
            this.navigationService = navigationService;
            this.taxonomyRetriever = taxonomyRetriever;
            this.currentLanguageRetriever = currentLanguageRetriever;
        }


        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            // Web page identification data
            var webPageData = webPageDataContextRetriever.Retrieve().WebPage;
            var webPageItemId = webPageData.WebPageItemID;
            var languageName = currentLanguageRetriever.Get();

            var productCategoryPage = await productCategoryRepository.GetProductCategory(webPageItemId, languageName, cancellationToken);
            var productCategoryTagIdentifiers = productCategoryPage.ProductCategoryTag.Select(t => t.Identifier);

            var products = (productCategoryPage.ProductType.Equals(PRODUCT_TYPE_SELECTION_OTHER_VALUE, StringComparison.InvariantCultureIgnoreCase)) ?
                            await productRepository.GetProducts(PRODUCT_CATEGORY_FIELD_NAME, await TagCollection.Create(productCategoryTagIdentifiers), languageName, cancellationToken) :
                            await productRepository.GetProducts(productCategoryPage.ProductType, PRODUCT_CATEGORY_FIELD_NAME, productCategoryTagIdentifiers, languageName, cancellationToken);

            var productPageUrls = await productPageRepository.GetProductPageUrls(products.Cast<IContentItemFieldsSource>(), languageName, cancellationToken);

            var productTagsTaxonomy = await taxonomyRetriever.RetrieveTaxonomy(DancingGoatTaxonomyConstants.PRODUCT_TAGS_TAXONOMY_NAME, languageName, cancellationToken);

            var categoryMenu = await navigationService.GetStoreNavigationItemViewModels(languageName, cancellationToken);

            return View(ProductListingViewModel.GetViewModel(productCategoryPage, products, productPageUrls, productTagsTaxonomy, categoryMenu, languageName));
        }
    }
}
