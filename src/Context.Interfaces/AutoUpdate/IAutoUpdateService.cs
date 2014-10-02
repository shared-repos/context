using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Context.Interfaces.AutoUpdate
{
    [Guid("019154CB-21CF-45aa-BE1A-C64C10D18EC8")]
    public interface IAutoUpdateService
    {
        void CheckForUpdates();

        IList<IUpdateHistory> History { get; }

        void Start();

        void Stop();

        event UpdateStatusEventHandler Ready;
    }
}
