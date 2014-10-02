using System;

namespace Context.Interfaces.Evaluator
{
    public interface IEvaluationScope
    {
        object this[IVariable variable] { get; }
    }
}
