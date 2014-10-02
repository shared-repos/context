using System;

namespace Context.Interfaces.UI
{
    [Flags]
    public enum SelectionTypes
    {
        Auto = 0,
        Add = 1,
        Primary = 2,
        Remove = 4,
        Replace = 8,
        Toggle = 16
    }
}
