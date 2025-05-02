using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Websites;
using CMS.Websites.Routing;

namespace DancingGoat.Models
{
    /// <summary>
    /// Represents a collection of product pages.
    /// </summary>
    public class ProductPageRepository : ContentRepositoryBase
    {
        private readonly ProductRepository productRepository;
        private readonly IWebPageUrlRetriever webPageUrlRetriever;
        private readonly ICacheDependencyBuilderFactory cacheDependencyBuilderFactory;


        /// <summary>
        /// Initializes new instance of <see cref="ProductPageRepository"/>.
        /// </summary>
        public ProductPageRepository(
            IWebsiteChannelContext websiteChannelContext,
            IContentQueryExecutor executor,
            IProgressiveCache cache,
            ProductRepository productRepository,
            IWebPageUrlRetriever webPageUrlRetriever,
            ICacheDependencyBuilderFactory cacheDependencyBuilderFactory)
            : base(websiteChannelContext, executor, cache)
        {
            this.productRepository = productRepository;
            this.webPageUrlRetriever = webPageUrlRetriever;
            this.cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
        }


        /// <summary>
        /// Returns <see cref="IProductFields"/> content item by its product url slug.
        /// </summary>
        public async Task<ProductPage> GetProductPage(int webPageItemId, string languageName, CancellationToken cancellationToken)
        {
            var queryBuilder = new ContentItemQueryBuilder()
                .ForContentType(ProductPage.CONTENT_TYPE_NAME,
                config => config
                        .ForWebsite(WebsiteChannelContext.WebsiteChannelName)
                        .WithLinkedItems(2)
                        .Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemID), webPageItemId))
                        .TopN(1))
                .InLanguage(languageName);

            var cacheSettings = new CacheSettings(5, WebsiteChannelContext.WebsiteChannelName, languageName, nameof(IProductFields), webPageItemId);

            var productPage = (await GetCachedQueryResult<ProductPage>(queryBuilder, new ContentQueryExecutionOptions(), cacheSettings, GetDependencyCacheKeys, cancellationToken))
                              .FirstOrDefault();

            return productPage;
        }


        /// <summary>
        /// Returns URLs of product pages linked to the specified reusable products.
        /// </summary>
        public async Task<Dictionary<int, string>> GetProductPageUrls(IEnumerable<IContentItemFieldsSource> linkedProducts, string languageName, CancellationToken cancellationToken)
        {
            var queryBuilder = new ContentItemQueryBuilder()
                .ForContentType(ProductPage.CONTENT_TYPE_NAME,
                    config => config
                        .Linking(nameof(ProductPage.ProductPageProduct), linkedProducts.Select(linkedProduct => linkedProduct.SystemFields.ContentItemID))
                        .WithLinkedItems(1)
                        .ForWebsite(WebsiteChannelContext.WebsiteChannelName, PathMatch.Children(DancingGoatConstants.PRODUCTS_PAGE_TREE_PATH))
                )
                .InLanguage(languageName);

            var cacheSettings = new CacheSettings(5, WebsiteChannelContext.WebsiteChannelName, nameof(GetProductPageUrls), string.Join("-", linkedProducts.Select(p => p.SystemFields.ContentItemID)), languageName);

            var productPages = await GetCachedQueryResult<ProductPage>(queryBuilder, new ContentQueryExecutionOptions(), cacheSettings, GetDependencyCacheKeys, cancellationToken);

            var productUrls = new Dictionary<int, string>();

            foreach (var linkedProduct in linkedProducts)
            {
                // Get product page by a linked product
                var productPage = productPages.FirstOrDefault(p => p.ProductPageProduct.FirstOrDefault()?.SystemFields.ContentItemID == linkedProduct.SystemFields.ContentItemID);
                if (productPage != null)
                {
                    productUrls[linkedProduct.SystemFields.ContentItemID] = (await webPageUrlRetriever.Retrieve(productPage, languageName, cancellationToken)).RelativePath;
                }
            }

            return productUrls;
        }


        private async Task<ISet<string>> GetDependencyCacheKeys(IEnumerable<ProductPage> productPages, CancellationToken cancellationToken)
        {
            var productItemsDependencies = await productRepository.GetDependencyCacheKeys(productPages.Select(p => p.ProductPageProduct.FirstOrDefault() as IProductFields), cancellationToken);

            var cacheDependencyBuilder = cacheDependencyBuilderFactory.Create();
            var cacheDependencies = cacheDependencyBuilder
                .ForWebPageItems()
                    .ByContentType<ProductPage>(WebsiteChannelContext.WebsiteChannelName).Builder()
                .ForInfoObjects<WebsiteChannelInfo>()
                    .ById(WebsiteChannelContext.WebsiteChannelID).Builder()
                .AddDependency(productItemsDependencies)
                .Build();

            return cacheDependencies.CacheKeys.ToHashSet();
        }
    }
}
