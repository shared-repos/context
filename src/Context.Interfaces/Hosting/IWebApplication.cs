using System;
using System.Net;

namespace Context.Interfaces.Hosting
{
    public interface IWebApplication
    {
        void ProcessRequest(HttpListenerContext context);
    }
}
