using System;
using System.Collections.Generic;

namespace Context.Interfaces.Data
{
    public interface IDataSource
    {
        IDataTable GetTable(string tableName);

        IDataTable Query(string queryText);

        void Execute(string command);

        void Flush();

        void Refresh();

        event DataSourceChangedHandler Changed;
    }

    public delegate void DataSourceChangedHandler();
}
