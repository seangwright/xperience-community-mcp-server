using System.Threading;
using System.Threading.Tasks;

namespace DancingGoat.Services;

/// <summary>
/// Service responsible for generating unique order numbers.
/// </summary>
public interface IOrderNumberGenerator
{
    /// <summary>
    /// Generates a new unique order number in the format "YYYY-N", where N is the sequential number for the year.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the generated order number.</returns>
    Task<string> GenerateOrderNumber(CancellationToken cancellationToken);
}
