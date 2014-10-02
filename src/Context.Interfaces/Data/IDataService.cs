using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Context.Interfaces.Data
{
    [Guid("38359600-0A45-491b-92F2-92A060CC6BAE")]
    public interface IDataService
    {
        IDataSource GetDataSource(string dataSource);
    }
}
