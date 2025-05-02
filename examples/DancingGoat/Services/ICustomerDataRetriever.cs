using CMS.Commerce;

using System.Threading;
using System.Threading.Tasks;

#pragma warning disable KXE0002 // Commerce feature is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

namespace DancingGoat.Services;

/// <summary>
/// Interface for retrieving customer data.
/// </summary>
public interface ICustomerDataRetriever
{
    /// <summary>
    /// Returns a customer object for the given member ID.
    /// </summary>
    Task<CustomerInfo> GetCustomerForMember(int memberId, CancellationToken cancellationToken);


    /// <summary>
    /// Returns a customer address object for the given customer ID.
    /// </summary>
    Task<CustomerAddressInfo> GetCustomerAddress(int customerId, CancellationToken cancellationToken);
}

#pragma warning restore KXE0002 // Commerce feature is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
