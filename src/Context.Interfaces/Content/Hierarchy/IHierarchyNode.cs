using System;
using System.Collections.Generic;
using System.Drawing;

namespace Context.Interfaces.Content.Hierarchy
{
    public interface IHierarchyNode
    {
        string Title { get; }

        string Description { get; }

        IContentItem Item { get; }

        IHierarchyNode ParentNode { get; }

        IHierarchy ParentHierarchy { get; }

        IHierarchy NestedHierarchy { get; }

        bool IsAlwaysLeaf { get; }

        bool IsFolder { get; }
    }
}
