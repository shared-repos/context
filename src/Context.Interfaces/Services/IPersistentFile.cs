using System;
using System.Collections.Generic;
using System.Text;

namespace Context.Interfaces.Services
{
    public interface IPersistentFile
    {
        string FilePath { get; }
        bool IsDirty { get; set; }
    }
}
