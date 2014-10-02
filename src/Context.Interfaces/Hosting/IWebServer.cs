using System;
using System.Collections.Generic;

namespace Context.Interfaces.Hosting
{
    public interface IWebServer
    {
        IList<IWebSite> Sites { get; }

        ServerState State { get; }

        void Restart();

        void Start();

        void Stop();
    }
}
