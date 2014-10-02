using System;
using System.Collections.Generic;
using System.Text;

namespace Context.Interfaces.Content
{
    public interface IContentMember
    {
        IContentType ParentType { get; }

        IContentRelation Relation { get; }

        string Name { get; }

        string Description { get; }

        string TypeName { get; }

        bool ReadOnly { get; }

        bool Required { get; }

        int Ordinal { get; }
    }
}
