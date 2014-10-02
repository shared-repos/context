using System;
using System.Collections.Generic;
using System.Text;

namespace Context.Core
{
    [Flags]
    internal enum RunOptions
    {
        None = 0,
        RunOnAttach = 1,
        RunOnDetach = 2,
        StopOnAttach = 4,
        StopOnDetach = 8,
        CheckSingleInstance = 16
    }
}
