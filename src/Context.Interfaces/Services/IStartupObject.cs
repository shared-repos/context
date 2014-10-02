using System;

namespace Context.Interfaces.Services
{
    public interface IStartupObject : IServiceManager, IDisposable
    {
        void Start();

        bool QueryClose();
    }
}
