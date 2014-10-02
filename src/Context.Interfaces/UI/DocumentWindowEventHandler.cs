using System;
using Context.Interfaces.UI.Documents;

namespace Context.Interfaces.UI
{
    public delegate void DocumentWindowEventHandler(IDocumentWindow window, IDocumentView view, IDocumentView oldView);
}
