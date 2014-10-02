using System;
using System.Collections.Generic;

namespace Context.Interfaces.UI.Documents
{
    public interface IHierarchyView
    {
        void AddHierarchy(IHierarchy hierarchy);
        void RemoveHierarchy(IHierarchy hierarchy);

        INode GetTopLevelNode(INode node);
        void CollapseNode(INode node);
        void DeleteNode(INode node);
        void EditNodeText(INode node);
        string GetNodePath(INode node);

        bool IsNodeExpanded(INode node);
        void ExpandNode(INode node);

        bool IsNodeSelected(INode node);
        void SelectNode(INode node, bool clearCurrent);
        void DeselectNode(INode node);

        bool IsNodeChecked(INode node);
        void SetNodeChecked(INode node, bool isChecked);

        bool AllowDefaultAction { get; set; }
        bool AllowNodeChecks { get; set; }
        bool MultipleSelect { get; set; }
        bool RecursiveCheck { get; set; }
        IList<INode> SelectedNodes { get; }

        event EventHandler SelectionChanged;
    }
}
