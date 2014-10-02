using System;
using System.Collections.Generic;

namespace Context.Interfaces.Content.Persistance
{
    public interface IPersisterFactory
    {
        IPersister CreatePersister();

        Type SourceType { get; }
    }
}
