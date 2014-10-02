using System;
using System.Collections.Generic;
using System.Data;

namespace Context.Interfaces.Content.Persistance
{
    public interface IPersisterFactories
    {
        DataTable GetFactoryClasses();

        IPersisterFactory GetFactory(string dataSourceName);

        IPersisterFactory GetFactory(DataRow persisterRow);
    }
}
