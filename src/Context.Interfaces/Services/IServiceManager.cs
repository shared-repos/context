using System;
using System.ComponentModel.Design;

namespace Context.Interfaces.Services
{
    public interface IServiceManager : IServiceContainer
    {
        object GetService(Guid serviceId);
    }
}
