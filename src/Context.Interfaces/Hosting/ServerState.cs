using System;

namespace Context.Interfaces.Hosting
{
    public enum ServerState
    {
        Stopped,
        Stopping,
        Starting,
        Started
    }
}
