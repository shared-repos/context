using System;

namespace Context.Interfaces.Rules
{
    public interface IRule
    {
        object[] Dependencies { get; }

        void ProcessRule();
    }
}
