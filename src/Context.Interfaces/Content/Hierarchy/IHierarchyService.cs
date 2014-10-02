using System;
using System.Collections.Generic;

namespace Context.Interfaces.Content.Hierarchy
{
    public interface IHierarchyService
    {
        IHierarchy CreateHierarchy(string moniker, Guid factoryId, bool createNew);

        void RegisterHierarchyFactory(Guid factoryId, IHierarchyFactory hierarchyFactory);

        void RemoveHierarchyFactory(Guid factoryId);
    }
}
