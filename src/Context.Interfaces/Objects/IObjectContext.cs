using System;
using System.Collections;
using Context.Interfaces.Content;
using Context.Interfaces.Content.Persistance;
using Context.Interfaces.Content.Query;

namespace Context.Interfaces.Objects
{
    public interface IObjectContext
    {
        IContentProvider ContentProvider { get; }

        IPersister Persister { get; }

        string DefaultNamespace { get; set; }

        // create
        object CreateObject(Type objectType);

        void AddObject(object dataObject);

        void MakePersistent(object dataObject);

        void MakeTransient(object dataObject);

        // retrieve
        object GetObject(IContentId objectId);

        IList GetObjects(IQuery query);

        // update
        void AcceptChanges(object dataObject);

        void RejectChanges(object dataObject);

        void SaveObject(object dataObject);

        void SaveObjects(IEnumerable objects);

        // delete
        void MarkDelete(object dataObject);

        void DeleteObject(object dataObject);

        // tracking
        void ReadComplete(RefreshMode refreshMode, object dataObject);

        void Refresh(RefreshMode refreshMode, object dataObject);

        void Refresh(RefreshMode refreshMode, IEnumerable objects);

        IContentId GetObjectId(object dataObject);

        ObjectState GetObjectState(object dataObject);

        ITransaction BeginTransaction(ICollection collection);

        event ObjectContextChangeEventHandler ItemAdded;

        event ObjectContextChangeEventHandler ItemRemoved;

        event ObjectContextItemStateChangeEventHandler ItemStateChange;
    }
}
