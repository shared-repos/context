using System;
using System.Collections.Generic;

namespace Context.Interfaces.Content
{
    public interface IContentRelation
    {
        IContentMember ParentMember { get; }

        bool IsCollection { get; }

        bool IsContained { get; }

        DeleteRule DeleteRule { get; }

        IContentType RelatedType { get; }

        IList<IContentRelation> Relations { get; }

        IList<IContentMember> Members { get; }

        IList<IContentMember> RelatedMembers { get; }
    }
}
