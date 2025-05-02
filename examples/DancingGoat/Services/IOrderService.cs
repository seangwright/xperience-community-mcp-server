using System.Threading.Tasks;
using System.Threading;

using DancingGoat.Models;
using DancingGoat.Commerce;

namespace DancingGoat.Services;

/// <summary>
/// Service for managing orders.
/// </summary>
public interface IOrderService
{
#pragma warning disable KXE0002 // Commerce feature is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    /// <summary>
    /// Creates an order based on the provided shopping cart and customer information.
    /// </summary>
    Task CreateOrder(ShoppingCartDataModel shoppingCart, CustomerDto customerDto, int memberId, CancellationToken cancellationToken);
#pragma warning restore KXE0002 // Commerce feature is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
