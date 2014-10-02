using System;

namespace Context.Interfaces.Services
{
    public interface IContext : IServiceProvider
    {
        string Name { get; }

        object this[string name] { get; set; }
    }
}
