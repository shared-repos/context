using System;
using System.Collections.Generic;
using System.Text;

namespace Context.Core
{
    internal class CommandGroup
    {
        private string name;
        private Guid id;
        private Dictionary<string, Command> commands;

        public CommandGroup(string name, Guid id)
        {
            this.name = name;
            this.id = id;
            this.commands = new Dictionary<string, Command>();
        }

        public Dictionary<string, Command> Commands
        {
            get
            {
                return this.commands;
            }
        }

        public Command this[string name]
        {
            get
            {
                Command command;
                commands.TryGetValue(name, out command);
                return command;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public Guid Id
        {
            get
            {
                return id;
            }
        }
    }
}
