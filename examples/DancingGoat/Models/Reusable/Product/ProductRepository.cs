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
    /// Represents a collection of product pages.
    /// </summary>
    public class ProductRepository : ContentRepositoryBase
    {
        private readonly ILinkedItemsDependencyAsyncRetriever linkedItemsDependencyRetriever;
        private readonly IInfoProvider<TaxonomyInfo> taxonomyInfoProvider;
        private readonly ICacheDependencyBuilderFactory cacheDependencyBuilderFactory;


        /// <summary>
        /// Initializes new instance of <see cref="ProductRepository"/>.
        /// </summary>
        public ProductRepository(
            IWebsiteChannelContext websiteChannelContext,
            IContentQueryExecutor executor,
            IProgressiveCache cache,
            ILinkedItemsDependencyAsyncRetriever linkedItemsDependencyRetriever,
            IInfoProvider<TaxonomyInfo> taxonomyInfoProvider,
            ICacheDependencyBuilderFactory cacheDependencyBuilderFactory)
            : base(websiteChannelContext, executor, cache)
        {
            this.linkedItemsDependencyRetriever = linkedItemsDependencyRetriever;
            this.taxonomyInfoProvider = taxonomyInfoProvider;
            this.cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
        }


        /// <summary>
        /// Returns list of <see cref="IProductFields"/> content items by their GUIDs.
        /// </summary>
        public async Task<IEnumerable<IProductFields>> GetProducts(ICollection<Guid> productGuids, string languageName, CancellationToken cancellationToken)
        {
            var queryBuilder = GetProductBaseQueryBuilder(languageName)
                                   .Parameters(query => query.Where(where =>
                                      where.WhereIn(nameof(IContentQueryDataContainer.ContentItemGUID), productGuids))
                                   );

            var cacheSettings = new CacheSettings(5, nameof(ProductRepository), nameof(GetProducts), nameof(productGuids), languageName, productGuids.Select(guid => guid.ToString()).Join("|"));

            return await GetCachedQueryResult<IProductFields>(queryBuilder, new ContentQueryExecutionOptions(), cacheSettings, GetDependencyCacheKeys, cancellationToken);
        }


        /// <summary>
        /// Returns list of <see cref="IProductFields"/> content items by their IDs.
        /// </summary>
        public async Task<IEnumerable<IProductFields>> GetProducts(ICollection<int> productIds, string languageName, CancellationToken cancellationToken)
        {
            var queryBuilder = GetProductBaseQueryBuilder(languageName)
                                   .Parameters(query => query.Where(where =>
                                      where.WhereIn(nameof(IContentQueryDataContainer.ContentItemID), productIds))
                                   );

            var cacheSettings = new CacheSettings(5, nameof(ProductRepository), nameof(GetProducts), nameof(productIds), languageName, productIds.Select(id => id.ToString()).Join("|"));

            return await GetCachedQueryResult<IProductFields>(queryBuilder, new ContentQueryExecutionOptions(), cacheSettings, GetDependencyCacheKeys, cancellationToken);
        }


        /// <summary>
        /// Returns list of <see cref="IProductFields"/> content items by their tags.
        /// </summary>
        public async Task<IEnumerable<IProductFields>> GetProducts(string contentTypeName, string taxonomyFieldName, IEnumerable<Guid> tagIdentifiers, string languageName, CancellationToken cancellationToken)
        {
            var queryBuilder = new ContentItemQueryBuilder()
                .ForContentType(contentTypeName, ct => ct.WithLinkedItems(1))
                .InLanguage(languageName)
                .InWorkspaces(DancingGoatConstants.COMMERCE_WORKSPACE_NAME);

            if (tagIdentifiers.Any())
            {
                var tagCollection = await TagCollection.Create(tagIdentifiers);
                queryBuilder.Parameters(query => query.Where(where => where.WhereContainsTags(taxonomyFieldName, tagCollection)));
            }

            var cacheSettings = new CacheSettings(5, nameof(ProductRepository), nameof(GetProducts), nameof(contentTypeName), contentTypeName, taxonomyFieldName, languageName, String.Join('|', tagIdentifiers));

            return await GetCachedQueryResult<IProductFields>(queryBuilder, new ContentQueryExecutionOptions(), cacheSettings, GetDependencyCacheKeys, cancellationToken);
        }


        /// <summary>
        /// Returns list of <see cref="IProductFields"/> content items by their tags.
        /// </summary>
        public async Task<IEnumerable<IProductFields>> GetProducts(string taxonomyFieldName, TagCollection tagCollection, string languageName, CancellationToken cancellationToken)
        {
            var queryBuilder = GetProductBaseQueryBuilder(languageName)
                                   .Parameters(query => query.Where(where =>
                                      where.WhereContainsTags(taxonomyFieldName, tagCollection))
                                   );

            var cacheSettings = new CacheSettings(5, nameof(ProductRepository), nameof(GetProducts), nameof(taxonomyFieldName), taxonomyFieldName, languageName, String.Join('|', tagCollection.TagIdentifiers));

            return await GetCachedQueryResult<IProductFields>(queryBuilder, new ContentQueryExecutionOptions(), cacheSettings, GetDependencyCacheKeys, cancellationToken);
        }


        private static ContentItemQueryBuilder GetProductBaseQueryBuilder(string languageName)
        {
            return new ContentItemQueryBuilder().ForContentTypes(ct =>
            {
                ct.OfReusableSchema(IProductFields.REUSABLE_FIELD_SCHEMA_NAME)
                  .WithContentTypeFields()
                  .WithLinkedItems(1);
            })
            .InLanguage(languageName)
            .InWorkspaces(DancingGoatConstants.COMMERCE_WORKSPACE_NAME);
        }


        public async Task<ISet<string>> GetDependencyCacheKeys(IEnumerable<IProductFields> products, CancellationToken cancellationToken)
        {
            var productItems = products.Cast<IContentItemFieldsSource>();
            var linkedDependencies = await linkedItemsDependencyRetriever.Get(productItems.Select(p => p.SystemFields.ContentItemID), 1, cancellationToken);

            var categoriesTaxonomyDependencies = await GetTaxonomyTagsCacheDependencyKey(DancingGoatTaxonomyConstants.PRODUCT_CATEGORIES_TAXONOMY_NAME);
            var manufacturersTaxonomyDependencies = await GetTaxonomyTagsCacheDependencyKey(DancingGoatTaxonomyConstants.PRODUCT_MANUFACTURERS_TAXONOMY_NAME);

            var cacheDependencyBuilder = cacheDependencyBuilderFactory.Create();
            var cacheDependencies = cacheDependencyBuilder
                .ForContentItems()
                    .ByReusableFieldSchema(IProductFields.REUSABLE_FIELD_SCHEMA_NAME).Builder()
                .ForInfoObjects<ContentLanguageInfo>()
                    .All().Builder()
                .AddDependency(linkedDependencies)
                .AddDependency(categoriesTaxonomyDependencies)
                .AddDependency(manufacturersTaxonomyDependencies)
                .Build();

            return cacheDependencies.CacheKeys.ToHashSet();
        }


        private async Task<string> GetTaxonomyTagsCacheDependencyKey(string taxonomyName)
        {
            var taxonomyID = (await taxonomyInfoProvider.GetAsync(taxonomyName))?.TaxonomyID;
            return taxonomyID == null ? null : CacheHelper.GetCacheItemName(null, TaxonomyInfo.OBJECT_TYPE, "byid", taxonomyID, "children");
        }
    }
}
