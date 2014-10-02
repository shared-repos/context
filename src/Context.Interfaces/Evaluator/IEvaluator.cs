using System;

namespace Context.Interfaces.Evaluator
{
    public interface IEvaluator
    {
        object Evaluate(object model);

        IEvaluationContext Context { get; }

        IEvaluationEngine Engine { get; }
    }
}
