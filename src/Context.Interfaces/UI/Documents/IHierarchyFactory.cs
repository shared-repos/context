using System;

namespace Context.Interfaces.UI.Documents
{
    public interface IHierarchyFactory
    {
        IHierarchy CreateHierarchy(string moniker, bool createNew);
    }
}
