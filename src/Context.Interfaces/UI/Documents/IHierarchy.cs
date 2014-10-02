using System;
using System.Collections.Generic;

namespace Context.Interfaces.UI.Documents
{
    public interface IHierarchy
    {
        event EventHandler<HierarchyAsyncErrorEventArgs> AsyncError;
        event EventHandler<NodeAddedEventArgs> NodeAdded;
        event EventHandler<NodeAddedEventArgs> NodeMoved;
        event EventHandler<NodeEventArgs> NodeChanged;
        event EventHandler<NodeEventArgs> NodeDeleted;
        event EventHandler<NodeEventArgs> Refreshing;
        event EventHandler<NodeEventArgs> Refreshed;

        void ClearHierarchy();
        INode AddItem(INode parentNode, string itemName, bool isFolder, bool isNew);
        void MoveNode(INode node, INode newParentNode);
        void DeleteNode(INode node);
        int GetChildCount(INode parentNode);
        IList<INode> GetChildren(INode parentNode);
        IList<INode> GetAllChildren(INode parentNode);
        void StartRefresh(INode node);
        void StopRefresh();

        IHierarchyView HierarchyView { get; set; }
        bool IsRefreshing { get; }
    }
}
