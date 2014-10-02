using System;
using System.Drawing;

namespace Context.Interfaces.UI
{
    public interface IToolWindow : IWindow
    {
        Guid Id { get; }
        Guid ControlId { get; }
        WindowBehavior Behavior { get; set; }
        WindowVisibility Visibility { get; set; }
        WindowLayout DefaultLayout { get; }
        IToolWindowContent Content { get; }
        object ContentControl { get; }
        Image Icon { get; }
        bool IsControlCreated { get; }
    }
}
