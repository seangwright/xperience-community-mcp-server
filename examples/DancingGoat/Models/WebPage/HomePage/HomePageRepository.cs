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

using Kentico.Content.Web.Mvc.Routing;

namespace DancingGoat.Models
{
    /// <summary>
    /// Represents a collection of home pages.
    /// </summary>
    public class HomePageRepository : ContentRepositoryBase
    {
        private readonly IWebPageLinkedItemsDependencyAsyncRetriever webPageLinkedItemsDependencyRetriever;
        private readonly IInfoProvider<WebsiteChannelInfo> websiteChannelInfoProvider;
        private readonly IPreferredLanguageRetriever preferredLanguageRetriever;


        /// <summary>
        /// Initializes new instance of <see cref="HomePageRepository"/>.
        /// </summary>
        public HomePageRepository(
            IWebsiteChannelContext websiteChannelContext,
            IContentQueryExecutor executor,
            IProgressiveCache cache,
            IWebPageLinkedItemsDependencyAsyncRetriever webPageLinkedItemsDependencyRetriever,
            IInfoProvider<WebsiteChannelInfo> websiteChannelInfoProvider,
            IPreferredLanguageRetriever preferredLanguageRetriever)
            : base(websiteChannelContext, executor, cache)
        {
            this.webPageLinkedItemsDependencyRetriever = webPageLinkedItemsDependencyRetriever;
            this.websiteChannelInfoProvider = websiteChannelInfoProvider;
            this.preferredLanguageRetriever = preferredLanguageRetriever;
        }


        /// <summary>
        /// Returns <see cref="HomePage"/> content item.
        /// </summary>
        public async Task<HomePage> GetHomePage(int webPageItemId, string languageName, CancellationToken cancellationToken = default)
        {
            var queryBuilder = GetQueryBuilder(webPageItemId, languageName);

            var cacheSettings = new CacheSettings(5, WebsiteChannelContext.WebsiteChannelName, nameof(HomePage), languageName);

            var result = await GetCachedQueryResult<HomePage>(queryBuilder, null, cacheSettings, GetDependencyCacheKeys, cancellationToken);

            return result.FirstOrDefault();
        }


        /// <summary>
        /// Returns <see cref="HomePage"/> of the website channel.
        /// </summary>
        public async Task<HomePage> GetChannelHomePage(CancellationToken cancellationToken = default)
        {
            var websiteChannelInfo = await websiteChannelInfoProvider.GetAsync(WebsiteChannelContext.WebsiteChannelID, cancellationToken);
            var languageName = preferredLanguageRetriever.Get();

            var queryBuilder = GetQueryBuilder(websiteChannelInfo.WebsiteChannelHomePage, languageName);

            var cacheSettings = new CacheSettings(60, WebsiteChannelContext.WebsiteChannelName, websiteChannelInfo.WebsiteChannelHomePage, languageName);

            var result = await GetCachedQueryResult<HomePage>(queryBuilder, null, cacheSettings, GetDependencyCacheKeys, cancellationToken);

            return result.FirstOrDefault();
        }


        private ContentItemQueryBuilder GetQueryBuilder(int webPageItemId, string languageName)
        {
            return new ContentItemQueryBuilder()
                    .ForContentType(HomePage.CONTENT_TYPE_NAME,
                        config => config
                                .WithLinkedItems(4)
                                .ForWebsite(WebsiteChannelContext.WebsiteChannelName)
                                .SetUrlLanguageBehavior(UrlLanguageBehavior.UseRequestedLanguage)
                                .Where(where => where.WhereEquals(nameof(IWebPageContentQueryDataContainer.WebPageItemID), webPageItemId))
                                .TopN(1))
                    .InLanguage(languageName);
        }


        private ContentItemQueryBuilder GetQueryBuilder(string homePageTreePath, string languageName)
        {
            return new ContentItemQueryBuilder()
                    .ForContentType(HomePage.CONTENT_TYPE_NAME,
                        config => config
                                .WithLinkedItems(4)
                                .ForWebsite(WebsiteChannelContext.WebsiteChannelName, PathMatch.Single(homePageTreePath))
                                .SetUrlLanguageBehavior(UrlLanguageBehavior.UseRequestedLanguage)
                                .TopN(1))
                    .InLanguage(languageName);
        }


        private async Task<ISet<string>> GetDependencyCacheKeys(IEnumerable<HomePage> homePages, CancellationToken cancellationToken)
        {
            var homePage = homePages.FirstOrDefault();

            if (homePage == null)
            {
                return new HashSet<string>();
            }

            return (await webPageLinkedItemsDependencyRetriever.Get(homePage.SystemFields.WebPageItemID, 4, cancellationToken))
                .Concat(GetCacheByGuidKeys(homePage.HomePageArticlesSection.Select(articlesSection => articlesSection.WebPageGuid)))
                .Append(CacheHelper.BuildCacheItemName(new[] { "webpageitem", "byid", homePage.SystemFields.WebPageItemID.ToString() }, false))
                .Append(CacheHelper.GetCacheItemName(null, WebsiteChannelInfo.OBJECT_TYPE, "byid", WebsiteChannelContext.WebsiteChannelID))
                .Append(CacheHelper.GetCacheItemName(null, ContentLanguageInfo.OBJECT_TYPE, "all"))
                .ToHashSet(StringComparer.InvariantCultureIgnoreCase);
        }


        private static IEnumerable<string> GetCacheByGuidKeys(IEnumerable<Guid> webPageGuids)
        {
            foreach (var guid in webPageGuids)
            {
                yield return CacheHelper.BuildCacheItemName(new[] { "webpageitem", "byguid", guid.ToString() }, false);
            }
        }
    }
}
