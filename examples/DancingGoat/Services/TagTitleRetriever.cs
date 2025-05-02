using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CMS.ContentEngine;

namespace DancingGoat.Services
{
    /// <inheritdoc cref="ITagTitleRetriever"/>
    internal class TagTitleRetriever : ITagTitleRetriever
    {
        private readonly ITaxonomyRetriever taxonomyRetriever;


        public TagTitleRetriever(ITaxonomyRetriever taxonomyRetriever)
        {
            this.taxonomyRetriever = taxonomyRetriever;
        }


        /// <inheritdoc/>
        public async Task<string> GetTagTitle(Guid tagIdentifier, string languageName, CancellationToken cancellationToken)
        {
            var tags = await taxonomyRetriever.RetrieveTags([tagIdentifier], languageName, cancellationToken);
            return tags.FirstOrDefault()?.Title;
        }
    }
}
