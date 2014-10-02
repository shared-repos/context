using System;

namespace Context.Interfaces.Evaluator
{
    public interface IValue<T>
    {
        T Value { get; set; }
    }
}
