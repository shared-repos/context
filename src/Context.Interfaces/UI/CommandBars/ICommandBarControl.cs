using System;
using System.Drawing;

namespace Context.Interfaces.UI.CommandBars
{
    public interface ICommandBarControl
    {
        ICommand Command { get; }
        ICommandBar ParentBar { get; }
        CommandBarControlType Type { get; }
        int Position { get; }
        bool IsDefault { get; }
        object Icon { get; set; }
        ShortcutEntry Shortcut { get; set; }
        string Name { get; set; }
        string Caption { get; set; }
        string ToolTip { get; set; }
        bool Checked { get; set; }
        bool Enabled { get; set; }
        bool Visible { get; set; }

        void Delete();

        event EventHandler Click;
    }
}
