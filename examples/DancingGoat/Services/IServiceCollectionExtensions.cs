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
            AddCommerceServices(services);

            services.AddSingleton<CurrentWebsiteChannelPrimaryLanguageRetriever>();
            services.AddSingleton<TagTitleRetriever>();
            services.AddSingleton<WebPageUrlProvider>();
        }


        private static void AddCommerceServices(IServiceCollection services)
        {
            services.AddSingleton<ContentItemEventHandlers>();

            services.AddSingleton<OrderService>();
            services.AddSingleton<CustomerDataRetriever>();
            services.AddSingleton<ProductNameProvider>();
            services.AddSingleton<OrderNumberGenerator>();
            services.AddSingleton<ProductSkuValidator>();
            services.AddSingleton<ProductParametersExtractor>();
            services.AddSingleton<ProductVariantsExtractor>();

            // Register extractors for product types
            services.AddSingleton<IProductTypeParametersExtractor, ProductManufacturerExtractor>();
            services.AddSingleton<IProductTypeParametersExtractor, CoffeeParametersExtractor>();
            services.AddSingleton<IProductTypeParametersExtractor, GrinderParametersExtractor>();
            services.AddSingleton<IProductTypeParametersExtractor, ProductTemplateAlphaSizeParametersExtractor>();

            // Register extractors for product type variants
            services.AddSingleton<IProductTypeVariantsExtractor, ProductTemplateAlphaSizeVariantsExtractor>();
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
