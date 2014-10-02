using System;

namespace Context.Interfaces.UI.Documents
{
    public interface IDocumentView
    {
        string Name { get; }
        string Caption { get; }
        object Control { get; }
        int ImageId { get; }
        string CommandBarContext { get; }
        string ShortcutContext { get; }
        bool Visible { get; set; }
    }
}
