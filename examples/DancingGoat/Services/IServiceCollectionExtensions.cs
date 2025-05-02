using DancingGoat.Commerce;
using DancingGoat.Models;
using DancingGoat.Services;
using DancingGoat.ViewComponents;

using Microsoft.Extensions.DependencyInjection;

namespace DancingGoat
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Injects DG services into the IoC container.
        /// </summary>
        public static void AddDancingGoatServices(this IServiceCollection services)
        {
            AddViewComponentServices(services);
            AddRepositories(services);

            services.AddSingleton<ICurrentWebsiteChannelPrimaryLanguageRetriever, CurrentWebsiteChannelPrimaryLanguageRetriever>();
            services.AddSingleton<IProductParametersExtractor, ProductParametersExtractor>();
            services.AddSingleton<IProductVariantsExtractor, ProductVariantsExtractor>();
            services.AddSingleton<ITagTitleRetriever, TagTitleRetriever>();
            services.AddSingleton<IWebPageUrlProvider, WebPageUrlProvider>();
            services.AddSingleton<IOrderService, OrderService>();
            services.AddSingleton<ICustomerDataRetriever, CustomerDataRetriever>();
            services.AddSingleton<ProductNameProvider>();
            services.AddSingleton<RazorPriceFormatter>();
            services.AddSingleton<IOrderNumberGenerator, OrderNumberGenerator>();
        }


        private static void AddRepositories(IServiceCollection services)
        {
            services.AddSingleton<SocialLinkRepository>();
            services.AddSingleton<ContactRepository>();
            services.AddSingleton<HomePageRepository>();
            services.AddSingleton<ArticlePageRepository>();
            services.AddSingleton<ArticlesSectionRepository>();
            services.AddSingleton<ConfirmationPageRepository>();
            services.AddSingleton<ImageRepository>();
            services.AddSingleton<CafeRepository>();
            services.AddSingleton<NavigationItemRepository>();
            services.AddSingleton<ContactsPageRepository>();
            services.AddSingleton<PrivacyPageRepository>();
            services.AddSingleton<LandingPageRepository>();
            services.AddSingleton<ProductSectionRepository>();
            services.AddSingleton<ProductPageRepository>();
            services.AddSingleton<ProductRepository>();
            services.AddSingleton<StoreRepository>();
            services.AddSingleton<ProductCategoryRepository>();
            services.AddSingleton<CountryStateRepository>();
        }


        private static void AddViewComponentServices(IServiceCollection services)
        {
            services.AddSingleton<NavigationService>();
        }
    }
}
