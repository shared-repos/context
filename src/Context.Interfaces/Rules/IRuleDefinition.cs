using System;

namespace Context.Interfaces.Rules
{
    public interface IRuleDefinition
    {
        string Name { get; set; }

        string Condition { get; set; }

        string[] Actions { get; set; }
    }
}
