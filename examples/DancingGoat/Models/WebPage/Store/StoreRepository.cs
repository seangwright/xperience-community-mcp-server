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
    /// Represents a store repository.
    /// </summary>
    public class StoreRepository : ContentRepositoryBase
    {
        private readonly ILinkedItemsDependencyAsyncRetriever linkedItemsDependencyRetriever;
        private readonly ICacheDependencyBuilderFactory cacheDependencyBuilderFactory;


        /// <summary>
        /// Initializes new instance of <see cref="StoreRepository"/>.
        /// </summary>
        public StoreRepository(IWebsiteChannelContext websiteChannelContext, IContentQueryExecutor executor, IProgressiveCache cache, ILinkedItemsDependencyAsyncRetriever linkedItemsDependencyRetriever,
            ICacheDependencyBuilderFactory cacheDependencyBuilderFactory)
            : base(websiteChannelContext, executor, cache)
        {
            this.linkedItemsDependencyRetriever = linkedItemsDependencyRetriever;
            this.cacheDependencyBuilderFactory = cacheDependencyBuilderFactory;
        }


        /// <summary>
        /// Returns <see cref="Store"/> content item.
        /// </summary>
        public async Task<Store> GetStore(int webPageItemId, string languageName, CancellationToken cancellationToken = default)
        {
            var queryBuilder = GetQueryBuilder(webPageItemId, languageName);

            var cacheSettings = new CacheSettings(5, WebsiteChannelContext.WebsiteChannelName, nameof(Store), languageName, webPageItemId);

            var result = await GetCachedQueryResult<Store>(queryBuilder, null, cacheSettings, GetDependencyCacheKeys, cancellationToken);

            return result.FirstOrDefault();
        }


        private ContentItemQueryBuilder GetQueryBuilder(int webPageItemId, string languageName)
        {
            return new ContentItemQueryBuilder()
                    .ForContentType(Store.CONTENT_TYPE_NAME,
                        config => config
                                .ForWebsite(WebsiteChannelContext.WebsiteChannelName)
                                .Where(where => where.WhereEquals(nameof(IWebPageContentQueryDataContainer.WebPageItemID), webPageItemId))
                                .TopN(1))
                    .InLanguage(languageName);
        }


        private async Task<ISet<string>> GetDependencyCacheKeys(IEnumerable<Store> storePages, CancellationToken cancellationToken)
        {
            var storePage = storePages.FirstOrDefault();

            if (storePage == null)
            {
                return new HashSet<string>();
            }

            var linkedDependencies = await linkedItemsDependencyRetriever.Get(storePage.SystemFields.ContentItemID, 1, cancellationToken);

            var cacheDependencyBuilder = cacheDependencyBuilderFactory.Create();
            var dependencies = cacheDependencyBuilder
                .ForWebPageItems()
                    .ById(storePage.SystemFields.WebPageItemID).Builder()
                .ForInfoObjects<WebsiteChannelInfo>()
                    .ById(WebsiteChannelContext.WebsiteChannelID).Builder()
                .ForInfoObjects<ContentLanguageInfo>()
                    .All().Builder()
                .AddDependency(linkedDependencies)
                .Build();

            return dependencies.CacheKeys.ToHashSet();
        }
    }
}
