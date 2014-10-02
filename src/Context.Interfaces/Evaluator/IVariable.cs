using System;

namespace Context.Interfaces.Evaluator
{
    public interface IVariable
    {
        string Name { get; }

        Type Type { get; }
    }
}
