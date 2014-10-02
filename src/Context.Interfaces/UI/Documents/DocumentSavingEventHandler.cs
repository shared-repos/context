using System;

namespace Context.Interfaces.UI.Documents
{
    public delegate void DocumentSavingEventHandler(IDocument document, ref SaveOptions saveOptions, ref string targetMoniker, ref bool cancelSaving);
}
