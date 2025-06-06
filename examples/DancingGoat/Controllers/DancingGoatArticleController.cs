﻿using System.Collections.Generic;
using System.Threading.Tasks;

using CMS.Websites;

using DancingGoat;
using DancingGoat.Controllers;
using DancingGoat.Models;

using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

using Microsoft.AspNetCore.Mvc;

[assembly: RegisterWebPageRoute(ArticlesSection.CONTENT_TYPE_NAME, typeof(DancingGoatArticleController), WebsiteChannelNames = new[] { DancingGoatConstants.WEBSITE_CHANNEL_NAME })]
[assembly: RegisterWebPageRoute(ArticlePage.CONTENT_TYPE_NAME, typeof(DancingGoatArticleController), WebsiteChannelNames = new[] { DancingGoatConstants.WEBSITE_CHANNEL_NAME }, ActionName = "Article")]

namespace DancingGoat.Controllers
{
    public class DancingGoatArticleController : Controller
    {
        private readonly ArticlePageRepository articlePageRepository;
        private readonly ArticlesSectionRepository articlesSectionRepository;
        private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
        private readonly IPreferredLanguageRetriever currentLanguageRetriever;


        public DancingGoatArticleController(
            ArticlePageRepository articlePageRepository,
            ArticlesSectionRepository articlesSectionRepository,
            IWebPageDataContextRetriever webPageDataContextRetriever,
            IPreferredLanguageRetriever currentLanguageRetriever)
        {
            this.articlePageRepository = articlePageRepository;
            this.articlesSectionRepository = articlesSectionRepository;
            this.webPageDataContextRetriever = webPageDataContextRetriever;
            this.currentLanguageRetriever = currentLanguageRetriever;
        }


        public async Task<IActionResult> Index()
        {
            var languageName = currentLanguageRetriever.Get();

            var webPage = webPageDataContextRetriever.Retrieve().WebPage;

            var articlesSection = await articlesSectionRepository.GetArticlesSection(webPage.WebPageItemID, languageName, HttpContext.RequestAborted);

            var articles = await articlePageRepository.GetArticlePages(articlesSection.SystemFields.WebPageItemTreePath, languageName, true, cancellationToken: HttpContext.RequestAborted);

            var models = new List<ArticleViewModel>();
            foreach (var article in articles)
            {
                var articleModel = ArticleViewModel.GetViewModel(article);
                models.Add(articleModel);
            }

            var model = ArticlesSectionViewModel.GetViewModel(articlesSection, models, articlesSection.GetUrl().RelativePath);

            return View(model);
        }


        public async Task<IActionResult> Article()
        {
            var languageName = currentLanguageRetriever.Get();
            var webPageItemId = webPageDataContextRetriever.Retrieve().WebPage.WebPageItemID;

            var article = await articlePageRepository.GetArticlePage(webPageItemId, languageName, HttpContext.RequestAborted);

            if (article is null)
            {
                return NotFound();
            }

            var model = ArticleDetailViewModel.GetViewModel(article);

            return new TemplateResult(model);
        }
    }
}
