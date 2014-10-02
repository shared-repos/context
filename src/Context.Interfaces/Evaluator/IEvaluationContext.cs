using System;

namespace Context.Interfaces.Evaluator
{
    public interface IEvaluationContext
    {
        IEvaluationScope CreateScope();

        IEvaluationScope GlobalScope { get; }

        IVariable this[string name] { get; }
    }
}
