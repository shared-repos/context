using System;

namespace Context.Interfaces.UI
{
    public interface IWindowFactory
    {
        IToolWindowContent CreateToolWindowControl(Guid controlId);
    }
}
