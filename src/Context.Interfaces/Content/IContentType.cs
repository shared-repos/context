using System;
using System.Collections.Generic;

namespace Context.Interfaces.Content
{
    public interface IContentType
    {
        string ID { get; }

        string TypeName { get; }

        string Description { get; }

        IContentType BaseType { get; }

        IList<IContentMember> Members { get; }
    }
}
