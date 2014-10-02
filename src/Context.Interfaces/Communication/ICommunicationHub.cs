using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Context.Interfaces.Communication
{
    [Guid("52D1D04D-2C27-44a1-91C2-9731BB96D615")]
    public interface ICommunicationHub : IDisposable
    {
        bool Push(string source, string address);

        bool Pull(string address, string target);

        void Queue(ICommunicationChannel channel, string address);
    }
}
