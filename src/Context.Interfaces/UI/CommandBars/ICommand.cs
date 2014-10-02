using System;
using System.Collections.Generic;
using System.Drawing;

namespace Context.Interfaces.UI.CommandBars
{
    public interface ICommand
    {
        string Name { get; }
        string Text { get; set; }
        string ToolTip { get; set; }
        CommandBarControlBehavior Behavior { get; }
        CommandBarControlType ControlType { get; }
        IList<string> Items { get; }
        string Value { get; set; }
        int SelectedIndex { get; set; }
        Guid ContextId { get; }
        string Group { get; }
        int ImageId { get; }
        Image Icon { get; }
        bool Checked { get; set; }
        bool Enabled { get; set; }
        bool Visible { get; set; }
    }
}
