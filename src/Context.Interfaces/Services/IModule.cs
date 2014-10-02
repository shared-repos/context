using System;

namespace Context.Interfaces.Services
{
    using Context.Interfaces.Configuration;

    public interface IModule : IServiceProvider
    {
        void Attach(IServiceManager manager);

        void Detach();

        object GetService(Guid serviceId);
    }
}
