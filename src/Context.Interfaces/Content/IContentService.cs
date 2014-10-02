using System;
using System.Collections.Generic;

namespace Context.Interfaces.Content
{
    public interface IContentService
    {
        IContentProvider GetContentProvider(Guid providerId);

        void RegisterContentProvider(Guid providerId, IContentProvider provider);

        void RemoveContentProvider(Guid providerId);
    }
}
