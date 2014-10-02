using System;

namespace Context.Interfaces.Evaluator
{
    public interface IEvaluationEngine
    {
        IEvaluator Parse(string text, IEvaluationContext context, bool lazyBinding);

        bool Cached { get; set; }

        void ClearCache();
    }
}
