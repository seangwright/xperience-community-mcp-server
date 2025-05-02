using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

using CMS.Websites;

namespace DancingGoat.Models
{
    public record RelatedPageViewModel(string Title, string TeaserUrl, string Summary, DateTime? PublicationDate, string Url)
    {
        /// <summary>
        /// Validates and maps <see cref="ArticlePage"/> or <see cref="ProductPage"/> to a <see cref="RelatedPageViewModel"/>.
        /// </summary>
        public static Task<RelatedPageViewModel> GetViewModel(IWebPageFieldsSource webPage, IWebPageUrlRetriever urlRetriever, string languageName)
        {
            if (webPage is ArticlePage article)
            {
                return GetViewModelFromArticlePage(article, urlRetriever, languageName);
            }
            else if (webPage is ProductPage productPage)
            {
                return GetViewModelFromProductPage(productPage, urlRetriever, languageName);
            }

            throw new ArgumentException($"Param {nameof(webPage)} must be {nameof(ArticlePage)} or {nameof(ProductPage)}");
        }


        private static async Task<RelatedPageViewModel> GetViewModelFromArticlePage(ArticlePage articlePage, IWebPageUrlRetriever urlRetriever, string languageName)
        {
            var url = await urlRetriever.Retrieve(articlePage, languageName);

            return new RelatedPageViewModel
            (
                articlePage.ArticleTitle,
                articlePage.ArticlePageTeaser.FirstOrDefault()?.ImageFile.Url,
                WebUtility.HtmlEncode(articlePage.ArticlePageSummary),
                articlePage.ArticlePagePublishDate,
                url.RelativePath
            );
        }


        private static async Task<RelatedPageViewModel> GetViewModelFromProductPage(ProductPage productPage, IWebPageUrlRetriever urlRetriever, string languageName)
        {
            var url = await urlRetriever.Retrieve(productPage, languageName);

            var product = productPage.ProductPageProduct.FirstOrDefault() as IProductFields;

            return new RelatedPageViewModel
            (
                product?.ProductFieldName,
                product?.ProductFieldImage.FirstOrDefault()?.ImageFile.Url ?? string.Empty,
                product?.ProductFieldDescription,
                null,
                url.RelativePath
            );
        }
    }
}
