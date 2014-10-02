using System;

namespace Context.Interfaces.UI.CommandBars
{
    [Flags]
    public enum CommandBarControlBehavior
    {
        None = 0,
        DefaultDisabled = 1,
        DefaultInvisible = 2,
        DefaultChecked = 4,
        DynamicText = 8,
        List = 16,
        BeginGroup = 32
    }
}
