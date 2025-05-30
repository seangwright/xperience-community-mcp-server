using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CMS.Websites;

using DancingGoat.Models;

using Kentico.Content.Web.Mvc.Routing;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace DancingGoat.ViewComponents
{
    /// <summary>
    /// Controller for article view component.
    /// </summary>
    public class ArticlesViewComponent : ViewComponent
    {
        private readonly ArticlePageRepository articlePageRepository;
        private readonly ArticlesSectionRepository articlesSectionRepository;
        private readonly IPreferredLanguageRetriever currentLanguageRetriever;

        private const int ARTICLES_PER_VIEW = 5;


        public ArticlesViewComponent(
            ArticlePageRepository articlePageRepository,
            ArticlesSectionRepository articlesSectionRepository,
            IPreferredLanguageRetriever currentLanguageRetriever)
        {
            this.articlePageRepository = articlePageRepository;
            this.articlesSectionRepository = articlesSectionRepository;
            this.currentLanguageRetriever = currentLanguageRetriever;
        }


        public async Task<ViewViewComponentResult> InvokeAsync(WebPageRelatedItem articlesSectionItem)
        {
            var languageName = currentLanguageRetriever.Get();

            var articlesSection = await articlesSectionRepository.GetArticlesSection(articlesSectionItem.WebPageGuid, languageName, HttpContext.RequestAborted);
            if (articlesSection == null)
            {
                return View("~/Components/ViewComponents/Articles/Default.cshtml", ArticlesSectionViewModel.GetViewModel(null, Enumerable.Empty<ArticleViewModel>(), string.Empty));
            }

            var articlePages = await articlePageRepository.GetArticlePages(articlesSection.SystemFields.WebPageItemTreePath,
                languageName, false, ARTICLES_PER_VIEW, HttpContext.RequestAborted);

            var models = new List<ArticleViewModel>();
            foreach (var article in articlePages)
            {
                var model = ArticleViewModel.GetViewModel(article);
                models.Add(model);
            }

            var viewModel = ArticlesSectionViewModel.GetViewModel(articlesSection, models, articlesSection.GetUrl().RelativePath);

            return View("~/Components/ViewComponents/Articles/Default.cshtml", viewModel);
        }
    }
}
