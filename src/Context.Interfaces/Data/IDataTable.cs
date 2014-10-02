using System;
using System.Collections.Generic;

namespace Context.Interfaces.Data
{
    public interface IDataTable : IList<IDataRow>
    {
        int FieldCount { get; }

        object GetField(int index);

        string GetFieldName(int index);

        IDataRow Create(object keyValue);

        IDataRow Find(object keyValue);

        void Update(object keyValue, IDataRow row);

        void Update(IDataRow row);

        void Delete(object keyValue);

        void Delete(IDataRow row);
    }
}
