using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Context.WinForms.UI.CommandBars
{
    internal class CommandBarMenu : CommandBar
    {
        private readonly MenuStrip item;

        public CommandBarMenu(CommandBarService commandBars, CommandBarControl parentControl, MenuStrip item, string displayName, int position)
            : base(commandBars, parentControl, item, displayName, position)
        {
            this.item = item;
        }
    }
}
