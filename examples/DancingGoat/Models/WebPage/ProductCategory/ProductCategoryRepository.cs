using System;
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
    /// Represents a product category repository.
    /// </summary>
    public class ProductCategoryRepository : ContentRepositoryBase
    {
        private readonly IWebPageLinkedItemsDependencyAsyncRetriever webPageLinkedItemsDependencyRetriever;
        private readonly IInfoProvider<TaxonomyInfo> taxonomyInfoProvider;


        /// <summary>
        /// Initializes new instance of <see cref="ProductCategoryRepository"/>.
        /// </summary>
        public ProductCategoryRepository(IWebsiteChannelContext websiteChannelContext, IContentQueryExecutor executor, IProgressiveCache cache,
            IWebPageLinkedItemsDependencyAsyncRetriever webPageLinkedItemsDependencyRetriever, IInfoProvider<TaxonomyInfo> taxonomyInfoProvider)
            : base(websiteChannelContext, executor, cache)
        {
            this.webPageLinkedItemsDependencyRetriever = webPageLinkedItemsDependencyRetriever;
            this.taxonomyInfoProvider = taxonomyInfoProvider;
        }


        /// <summary>
        /// Returns <see cref="ProductCategory"/> content item.
        /// </summary>
        public async Task<ProductCategory> GetProductCategory(int webPageItemId, string languageName, CancellationToken cancellationToken)
        {
            var queryBuilder = GetQueryBuilder(webPageItemId, languageName);

            var cacheSettings = new CacheSettings(5, WebsiteChannelContext.WebsiteChannelName, nameof(ProductCategory), languageName, webPageItemId);

            var result = await GetCachedQueryResult<ProductCategory>(queryBuilder, null, cacheSettings, GetDependencyCacheKeys, cancellationToken);

            return result.FirstOrDefault();
        }


        /// <summary>
        /// Returns a collection of all <see cref="ProductCategory"/> content items in the given <paramref name="languageName"/>.
        /// </summary>
        public async Task<ICollection<ProductCategory>> GetProductCategories(string languageName, CancellationToken cancellationToken)
        {
            var queryBuilder = GetQueryBuilder(languageName);

            var cacheSettings = new CacheSettings(5, WebsiteChannelContext.WebsiteChannelName, nameof(ProductCategory), languageName);

            var result = await GetCachedQueryResult<ProductCategory>(queryBuilder, null, cacheSettings, GetDependencyCacheKeys, cancellationToken);

            return result.ToList();
        }


        private ContentItemQueryBuilder GetQueryBuilder(string languageName)
        {
            return new ContentItemQueryBuilder()
                    .ForContentType(ProductCategory.CONTENT_TYPE_NAME,
                        config => config
                                .ForWebsite(WebsiteChannelContext.WebsiteChannelName))
                    .InLanguage(languageName);
        }


        private ContentItemQueryBuilder GetQueryBuilder(int webPageItemId, string languageName)
        {
            return new ContentItemQueryBuilder()
                    .ForContentType(ProductCategory.CONTENT_TYPE_NAME,
                        config => config
                                .ForWebsite(WebsiteChannelContext.WebsiteChannelName)
                                .Where(where => where.WhereEquals(nameof(IWebPageContentQueryDataContainer.WebPageItemID), webPageItemId))
                                .TopN(1))
                    .InLanguage(languageName);
        }


        private async Task<ISet<string>> GetDependencyCacheKeys(IEnumerable<ProductCategory> productCategories, CancellationToken cancellationToken)
        {
            var taxonomyDependencyKeys = new HashSet<string>();
            foreach (var category in productCategories)
            {
                var dependencyKey = await GetTaxonomyTagsCacheDependencyKey(category.ProductType);
                taxonomyDependencyKeys.Add(dependencyKey);
            }

            var productCategory = productCategories.FirstOrDefault();
            if (productCategory != null)
            {
                taxonomyDependencyKeys.Add(CacheHelper.BuildCacheItemName(new[] { "webpageitem", "byid", productCategory.SystemFields.WebPageItemID.ToString() }, false));
            }

            taxonomyDependencyKeys.Add(CacheHelper.GetCacheItemName(null, WebsiteChannelInfo.OBJECT_TYPE, "byid", WebsiteChannelContext.WebsiteChannelID));
            taxonomyDependencyKeys.Add(CacheHelper.GetCacheItemName(null, ContentLanguageInfo.OBJECT_TYPE, "all"));

            return taxonomyDependencyKeys;
        }


        private async Task<string> GetTaxonomyTagsCacheDependencyKey(string taxonomyName)
        {
            var taxonomyID = (await taxonomyInfoProvider.GetAsync(taxonomyName))?.TaxonomyID;
            return taxonomyID == null ? null : CacheHelper.GetCacheItemName(null, TaxonomyInfo.OBJECT_TYPE, "byid", taxonomyID, "children");
        }
    }
}
