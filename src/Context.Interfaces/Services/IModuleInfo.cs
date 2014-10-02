using System;

namespace Context.Interfaces.Services
{
    public interface IModuleInfo : IPropertyProvider
    {
        Guid Id { get; }
        string Name { get; }
        string Description { get; }
        string AssemblyName { get; }
        string ClassName { get; }
        string Path { get; }
        string ServiceName { get; }
    }
}
