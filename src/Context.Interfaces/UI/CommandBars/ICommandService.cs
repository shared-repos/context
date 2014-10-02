using System;

namespace Context.Interfaces.UI.CommandBars
{
    public interface ICommandService : ICommandTarget
    {
        void RegisterCommandGroup(string commandGroup, Guid id);
        Guid GetCommandGroupId(string commandGroup);

        Guid DefaultContextId { get; set; }
        void AttachCommandTarget(Guid contextId, ICommandTarget target);

        ICommand AddCommand(string commandName, string commandGroup, string displayText, string toolTipText, int imageId, CommandBarControlType controlType, CommandBarControlBehavior behavior, Guid contextId);
        bool RemoveCommand(ICommand command);

        ICommand FindCommand(string commandName, string commandGroup);
        ICommand[] ListCommands();
    }
}
