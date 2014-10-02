using System;

namespace Context.Interfaces.UI
{
    public interface IWindow : IDisposable
    {
        void Close();
        void Show(bool activate);

        string Caption { get; set; }
        object SelectedObject { get; set; }
        bool Visible { get; set; }
    }
}
