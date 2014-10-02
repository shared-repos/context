using System;
using System.Collections.Generic;

namespace Context.Interfaces.UI.Documents
{
    public interface IDocumentService
    {
        IDocument CreateDocument(string moniker, string view, Guid factoryId, bool createNew);
        IDocument CreateDocument(IHierarchy hierarchy, INode item, string view, Guid factoryId);

        bool SaveDocument(IDocument document, SaveOptions saveOptions, string newMoniker);
        bool SaveDocuments(IList<IDocument> documents, SaveOptions saveOptions);
        bool CloseDocument(IDocument document, SaveOptions saveOptions);
        bool CloseDocuments(IList<IDocument> documents, SaveOptions saveOptions);

        void RegisterDocumentFactory(Guid factoryId, IDocumentFactory factory);

        IDocument FindDocument(string moniker);
        IDocument[] ListDocuments();
        IDocument ActiveDocument { get; set; }

        event EventHandler ActiveDocumentChanged;
        event DocumentEventHandler DocumentActivated;
        event DocumentClosingEventHandler DocumentClosing;
        event DocumentEventHandler DocumentClosed;
        event DocumentEventHandler DocumentDeactivated;
        event DocumentSavingEventHandler DocumentSaving;
        event DocumentSavedEventHandler DocumentSaved;
        event DocumentEventHandler DocumentChanged;
        event DocumentEventHandler DocumentReloaded;
    }
}
