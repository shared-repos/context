using System;
using System.Collections.Generic;

namespace Context.Interfaces.Content.Hierarchy
{
    public interface IHierarchyFactory
    {
        IHierarchy CreateHierarchy(string moniker, bool createNew);
    }
}
