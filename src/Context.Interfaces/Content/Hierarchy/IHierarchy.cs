using System;
using System.Collections.Generic;

namespace Context.Interfaces.Content.Hierarchy
{
    public interface IHierarchy
    {
        void ClearHierarchy();

        IHierarchyNode AddNode(IHierarchyNode parentNode, string title, bool isFolder, bool isNew);

        void MoveNode(IHierarchyNode node, IHierarchyNode newParentNode);

        void DeleteNode(IHierarchyNode node);

        int GetChildCount(IHierarchyNode parentNode);

        IList<IHierarchyNode> GetChildren(IHierarchyNode parentNode);

        IList<IHierarchyNode> GetAllChildren(IHierarchyNode parentNode);

        IContentProvider ContentProvider { get; }
    }
}
