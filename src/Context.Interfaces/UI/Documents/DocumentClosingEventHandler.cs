using System;

namespace Context.Interfaces.UI.Documents
{
    public delegate void DocumentClosingEventHandler(IDocument document, ref bool cancelClose);
}
