using System;
using System.Collections.Generic;

namespace Context.Interfaces.Content.Query
{
    public interface IContentId : IQuery
    {
        object KeyValue { get; }
    }
}
