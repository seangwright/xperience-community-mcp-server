using System;
using System.Threading;
using System.Threading.Tasks;

namespace DancingGoat.Services
{
    /// <summary>
    /// Tag title retriver to get title of a tag.
    /// </summary>
    public interface ITagTitleRetriever
    {
        /// <summary>
        /// Get title of a tag based on the tag identifier.
        /// </summary>
        /// <param name="tagIdentifier">Tag indentifier to retrive from database.</param>
        /// <param name="languageName">Language name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Received title if tag exists, null otherwise.</returns>
        Task<string> GetTagTitle(Guid tagIdentifier, string languageName, CancellationToken cancellationToken);
    }
}
