using System.Threading;
using System.Threading.Tasks;

namespace DancingGoat.Services
{
    /// <summary>
    /// Provides URLs of the web pages in the Dancing Goat sample application.
    /// </summary>
    public interface IWebPageUrlProvider
    {
        Task<string> CheckoutPageUrl(string languageName = null, CancellationToken cancellationToken = default);


        Task<string> HomePageUrl(string languageName = null, CancellationToken cancellationToken = default);


        Task<string> ShoppingCartPageUrl(string languageName = null, CancellationToken cancellationToken = default);


        Task<string> StorePageUrl(string languageName = null, CancellationToken cancellationToken = default);
    }
}
