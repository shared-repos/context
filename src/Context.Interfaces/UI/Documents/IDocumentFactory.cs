using System;

namespace Context.Interfaces.UI.Documents
{
    public interface IDocumentFactory
    {
        IDocument CreateDocument(string moniker, bool createNew);
        IDocument CreateDocument(IHierarchy hierarchy, INode item);
    }
}
