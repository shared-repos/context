using System;

namespace Context.Interfaces.UI.CommandBars
{
    public interface ICommandTarget
    {
        CommandStatus InvokeCommand(object sender, ICommand command, ICommandBarControl control);
        CommandStatus UpdateCommand(object sender, ICommand command);
    }
}
