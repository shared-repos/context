using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Context.WinForms.UI.CommandBars
{
    internal class CommandBarToolbar : CommandBar
    {
        public CommandBarToolbar(CommandBarService commandBars, CommandBarControl parentControl, ToolStrip toolStrip, string displayName, int position)
            : base(commandBars, parentControl, toolStrip, displayName, position)
        {
        }
    }
}
