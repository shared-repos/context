using System;
using System.Collections.Generic;

namespace Context.Interfaces.Data
{
    public interface IDataRow
    {
        object this[object field] { get; set; }

        object this[int fieldIndex] { get; set; }
    }
}
