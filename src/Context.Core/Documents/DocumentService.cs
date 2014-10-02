using System;
using System.Collections.Generic;
using Context.Interfaces.UI.Documents;
using Context.Interfaces.Services;
using Context.Interfaces.UI;

namespace Context.Core
{
    internal class DocumentService : IDocumentService
    {
        private readonly IServiceManager manager;
        private readonly Dictionary<Guid, IDocumentFactory> registeredFactories;

        public DocumentService(IServiceManager manager)
        {
            this.manager = manager;
            this.registeredFactories = new Dictionary<Guid, IDocumentFactory>();
        }

        #region IDocumentService Members

        public IDocument[] ListDocuments()
        {
            throw new NotImplementedException();
        }

        public IDocument ActiveDocument
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        private IDocumentFactory GetDocumentFactory(Guid factoryId)
        {
            IDocumentFactory factory;
            if (!registeredFactories.TryGetValue(factoryId, out factory))
            {
                IModuleManager modules = (IModuleManager)manager.GetService(typeof(IModuleManager));
                if (modules == null)
                {
                    return null;
                }

                IModule module = modules.LoadModule(factoryId);
                if (module == null)
                {
                    return null;
                }

                factory = module as IDocumentFactory;
                if (factory == null)
                {
                    factory = (IDocumentFactory)module.GetService(typeof(IDocumentFactory));
                }
                if (factory != null)
                {
                    registeredFactories[factoryId] = factory;
                }
            }
            return factory;
        }

        public IDocument CreateDocument(string moniker, string view, Guid factoryId, bool createNew)
        {
            IDocumentFactory factory = GetDocumentFactory(factoryId);
            if (factory != null)
            {
                IDocument document = factory.CreateDocument(moniker, createNew);
                OpenDocument(document, view);
                return document;
            }
            return null;
        }

        public IDocument CreateDocument(IHierarchy hierarchy, INode item, string view, Guid factoryId)
        {
            IDocumentFactory factory = GetDocumentFactory(factoryId);
            if (factory != null)
            {
                IDocument document = factory.CreateDocument(hierarchy, item);
                OpenDocument(document, view);
                return document;
            }
            return null;
        }

        private void OpenDocument(IDocument document, string view)
        {
            IWindowService windows = (IWindowService)manager.GetService(typeof(IWindowService));
            if (windows != null)
            {
                windows.CreateDocumentWindow(document, view);
            }
        }

        public IDocument FindDocument(string moniker)
        {
            throw new NotImplementedException();
        }

        public bool SaveDocument(IDocument document, SaveOptions saveOptions, string newMoniker)
        {
            throw new NotImplementedException();
        }

        public bool SaveDocuments(IList<IDocument> documents, SaveOptions saveOptions)
        {
            throw new NotImplementedException();
        }

        public bool CloseDocument(IDocument document, SaveOptions saveOptions)
        {
            throw new NotImplementedException();
        }

        public bool CloseDocuments(IList<IDocument> documents, SaveOptions saveOptions)
        {
            throw new NotImplementedException();
        }

        public void RegisterDocumentFactory(Guid factoryId, IDocumentFactory factory)
        {
            registeredFactories[factoryId] = factory;
        }

        internal void OnActiveDocumentChanged(object sender)
        {
            if (ActiveDocumentChanged != null)
            {
                ActiveDocumentChanged(sender, EventArgs.Empty);
            }
        }

        internal void OnDocumentActivated(IDocument document)
        {
            if (DocumentActivated != null)
            {
                DocumentActivated(document);
            }
        }

        internal void OnDocumentClosing(IDocument document, ref bool cancelClose)
        {
            if (DocumentClosing != null)
            {
                DocumentClosing(document, ref cancelClose);
            }
        }

        internal void OnDocumentClosed(IDocument document)
        {
            if (DocumentClosed != null)
            {
                DocumentClosed(document);
            }
        }

        internal void OnDocumentDeactivated(IDocument document)
        {
            if (DocumentDeactivated != null)
            {
                DocumentDeactivated(document);
            }
        }

        internal void OnDocumentSaving(IDocument document, ref SaveOptions saveOptions, ref string targetMoniker, ref bool cancelSaving)
        {
            if (DocumentSaving != null)
            {
                DocumentSaving(document, ref saveOptions, ref targetMoniker, ref cancelSaving);
            }
        }

        internal void OnDocumentSaved(IDocument document, string oldMoniker)
        {
            if (DocumentSaved != null)
            {
                DocumentSaved(document, oldMoniker);
            }
        }

        internal void OnDocumentChanged(IDocument document)
        {
            if (DocumentChanged != null)
            {
                DocumentChanged(document);
            }
        }

        internal void OnDocumentReloaded(IDocument document)
        {
            if (DocumentReloaded != null)
            {
                DocumentReloaded(document);
            }
        }

        public event EventHandler ActiveDocumentChanged;

        public event DocumentEventHandler DocumentActivated;

        public event DocumentClosingEventHandler DocumentClosing;

        public event DocumentEventHandler DocumentClosed;

        public event DocumentEventHandler DocumentDeactivated;

        public event DocumentSavingEventHandler DocumentSaving;

        public event DocumentSavedEventHandler DocumentSaved;

        public event DocumentEventHandler DocumentChanged;

        public event DocumentEventHandler DocumentReloaded;

        #endregion

    }
}
