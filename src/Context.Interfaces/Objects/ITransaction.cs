using System;
using System.Collections;

namespace Context.Interfaces.Objects
{
    public interface ITransaction
    {
        void AddObject(object dataObject);

        void AddObjectList(IList list, object listOwner);

        bool Contains(object obj);

        void Commit();

        void Cancel();

        void Rollback();

        event EventHandler Finish;

        event EventHandler Changed;

        IObjectContext Context { get; }

        TransactionState State { get; }

        int Id { get; }

        IList Objects { get; }
    }
}
