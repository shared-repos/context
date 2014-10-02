using System;
using Context.Interfaces.Content;
using Context.Interfaces.Content.Persistance;

namespace Context.Interfaces.Objects
{
    public interface IObjectContextFactory
    {
        IObjectContext CreateContext(IContentProvider provider, IPersister persister);
    }
}
