using System;
using Context.Interfaces.UI;
using System.Collections.Generic;

namespace Context.Interfaces.UI.Documents
{
    public interface IHierarchyService
    {
        IHierarchy CreateHierarchy(string moniker, Guid factoryId, bool createNew, Guid windowId);

        void RegisterHierarchyFactory(Guid factoryId, IHierarchyFactory hierarchyFactory);

        IHierarchy CurrentHierarchy { get; set; }

        event HierarchyEventHandler CurrentHierarchyChanged;
    }
}
