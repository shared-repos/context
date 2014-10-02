using System;

namespace Context.Interfaces.UI
{
    public interface IToolWindowContent
    {
        void OnToolWindowClosed();

        IToolWindow Window { get; set; }
    }
}
