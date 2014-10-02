using System;
using System.Drawing;

namespace Context.Interfaces.UI.Documents
{
    public interface INode
    {
        // Hierarchy
        object DocumentObject { get; }
        object NodeObject { get; }
        bool CanDeleteNode { get; }
        bool CanEditText { get; }
        bool HasNestedHierarchy { get; }
        bool HasReferences { get; }
        bool IsAlwaysLeaf { get; }
        IHierarchy NestedHierarchy { get; }
        IHierarchy ParentHierarchy { get; }
        INode ParentNode { get; }

        // View state
        Image Icon { get; }
        string Text { get; }
        string ToolTip { get; }
        string EditText { get; set; }
        bool IsExpanded { get; set; }
    }
}
