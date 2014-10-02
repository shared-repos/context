using System;
using System.Runtime.InteropServices;
using Context.Interfaces.UI.Documents;

namespace Context.Interfaces.UI
{
    [Guid("8A386633-2C76-47b1-A44E-466097681144")]
    public interface IWindowService
    {
        IDocumentWindow CreateDocumentWindow(IDocument document, string view);
        IToolWindow CreateToolWindow(Guid windowId, Guid controlId, IWindowFactory factory, string caption, int imageId, WindowVisibility visibility, WindowLayout layout, WindowBehavior behavior);
        IToolWindow FindToolWindow(Guid windowId);
        IWindow GetParentWindow(object control);

        IWindow ActiveWindow { get; set; }
        IWindow this[int index] { get; }
        int Count { get; }

        IDocumentWindow ActiveDocumentWindow { get; }
        IDocumentWindow[] GetDocumentWindows { get; }

        string CurrentTheme { get; }

        event WindowEventHandler WindowCreated;
        event WindowEventHandler WindowActivated;
        event WindowEventHandler WindowDeactivated;
        event DocumentWindowEventHandler TabbedWindowClosed;
        event DocumentWindowEventHandler WindowActiveViewChanged;
    }
}
