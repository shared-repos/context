using System;
using System.Collections.Generic;

namespace Context.Interfaces.Rules
{
    public interface IRuleEngine
    {
        bool Active { get; set; }

        IList<IRule> Rules { get; }

        void Trigger(object state);

        void ProcessAllRules();
    }
}
