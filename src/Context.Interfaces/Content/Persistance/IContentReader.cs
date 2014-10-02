using System;
using System.Collections.Generic;
using Context.Interfaces.Content.Query;

namespace Context.Interfaces.Content.Persistance
{
    public interface IContentReader
    {
        void Close();

        bool Read();

        IContentId GetContentId();

        IContentItem GetContentItem();

        IContentItem GetNestedItem(IRelationMapping relationMapping);

        IContentReader GetNestedReader(IRelationMapping relationMapping);

        IList<IContentItem> GetNestedCollection(IRelationMapping relationMapping);

        void Seek(int index);

        int Count { get; }

        bool IsClosed { get; }

        bool IsLazy { get; }

        IMemberMapping[] MemberMappings { get; }

        IRelationMapping[] RelationMappings { get; }

        bool Cached { get; }

        IContentType Type { get; }
    }
}
