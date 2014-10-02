using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Context.Interfaces.Services
{
    [Guid("1A84F3CF-5FA4-4bf2-A85D-8C57BEA18F50")]
    public interface IContextService
    {
        IContext Current { get; }

        IContext GetContext(string scopeName);

        IDisposable CreateScope(string scopeName);
    }
}
