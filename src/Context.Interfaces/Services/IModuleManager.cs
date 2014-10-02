using System;

namespace Context.Interfaces.Services
{
    public interface IModuleManager
    {
        IModuleInfo[] GetModuleList();
        Guid[] ListModuleServices(Guid moduleId);
        bool IsModuleRegistered(Guid moduleId);
        bool IsModuleLoaded(Guid moduleId);
        IModule LoadModule(Guid moduleId);
        void UnloadModule(Guid moduleId);

        event ClosingEventHandler Closing;
    }
}
