using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CMS.ContentEngine;

using DancingGoat;
using DancingGoat.Controllers;
using DancingGoat.Models;
using DancingGoat.ViewComponents;

using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;

using Microsoft.AspNetCore.Mvc;

using static DancingGoat.ProductTagTaxonomyConstants;

[assembly: RegisterWebPageRoute(Store.CONTENT_TYPE_NAME, typeof(DancingGoatStoreController), WebsiteChannelNames = [DancingGoatConstants.WEBSITE_CHANNEL_NAME])]

namespace DancingGoat.Controllers
{
    public class DancingGoatStoreController : Controller
    {
        private readonly StoreRepository storeRepository;
        private readonly NavigationService navigationService;
        private readonly ProductRepository productRepository;
        private readonly ProductPageRepository productPageRepository;
        private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
        private readonly ITaxonomyRetriever taxonomyRetriever;
        private readonly IPreferredLanguageRetriever currentLanguageRetriever;


        private const string PRODUCT_TAGS_FIELD_NAME = "ProductFieldTags";
        private readonly string[] PRODUCT_TAGS_TO_DISPLAY = [TAG_NAME_BESTSELLER, TAG_NAME_HOT_TIPS];


        public DancingGoatStoreController(StoreRepository storePageRepository, ProductRepository productRepository, ProductPageRepository productPageRepository,
        NavigationService navigationService, IWebPageDataContextRetriever webPageDataContextRetriever, ITaxonomyRetriever taxonomyRetriever, IPreferredLanguageRetriever currentLanguageRetriever)
        {
            this.storeRepository = storePageRepository;
            this.navigationService = navigationService;
            this.productRepository = productRepository;
            this.productPageRepository = productPageRepository;
            this.webPageDataContextRetriever = webPageDataContextRetriever;
            this.taxonomyRetriever = taxonomyRetriever;
            this.currentLanguageRetriever = currentLanguageRetriever;
        }


        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            // Web page identification data
            var webPageData = webPageDataContextRetriever.Retrieve().WebPage;
            var webPageItemId = webPageData.WebPageItemID;
            var languageName = currentLanguageRetriever.Get();

            var storePage = await storeRepository.GetStore(webPageItemId, languageName, cancellationToken);

            var products = await productRepository.GetProducts(PRODUCT_TAGS_FIELD_NAME, await TagCollection.Create(PRODUCT_TAGS_TO_DISPLAY), languageName, cancellationToken);

            var productPageUrls = await productPageRepository.GetProductPageUrls(products.Cast<IContentItemFieldsSource>(), languageName, cancellationToken);

            var categoryMenu = await navigationService.GetStoreNavigationItemViewModels(languageName, cancellationToken);

            var productTagsTaxonomy = await taxonomyRetriever.RetrieveTaxonomy(DancingGoatTaxonomyConstants.PRODUCT_TAGS_TAXONOMY_NAME, languageName, cancellationToken);

            return View(StoreViewModel.GetViewModel(storePage, products, productPageUrls, PRODUCT_TAGS_TO_DISPLAY, productTagsTaxonomy, languageName, categoryMenu));
        }
    }
}
