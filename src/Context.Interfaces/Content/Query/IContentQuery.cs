using System;
using System.Collections.Generic;

namespace Context.Interfaces.Content.Query
{
    public interface IContentQuery : IQuery
    {
        IContentType SourceType { get; }

        string Criteria { get; }

        string Aggregate { get; }

        string Sort { get; }

        string Having { get; }
    }
}
