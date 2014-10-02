using System;
using System.Collections.Generic;

namespace Context.Interfaces.UI.Documents
{
    public interface IDocument
    {
        void Reload();
        void Load(string moniker);
        void Save();

        void Activate(IDocumentView view);
        void OnActiveViewChanged(IDocumentView view);
        void OnActiveViewChanging(IDocumentView view, ref bool cancel);
        void OpenView(IDocumentView view);
        void CloseView(IDocumentView view);

        void Close();
        void OnClosing(ref SaveOptions saveOptions);

        void Redo();
        void Undo();

        IHierarchy Hierarchy { get; set; }
        INode Node { get; set; }

        string Name { get; set; }
        string Moniker { get; }
        string InitialView { get; }
        int ImageId { get; }
        bool IsNew { get; }
        bool Modified { get; }
        bool ReadOnly { get; set; }
        bool IsVirtual { get; set; }
        bool CanRedo { get; }
        bool CanUndo { get; }

        IDocumentView ActiveView { get; set; }
        IList<IDocumentView> Views { get; }
    }
}
