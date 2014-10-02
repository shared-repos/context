using System;
using Context.Interfaces.UI.Documents;

namespace Context.Interfaces.UI
{
    public interface IDocumentWindow : IWindow
    {
        IDocumentView ActiveView { get; set; }
        IDocument Document { get; }
    }
}
