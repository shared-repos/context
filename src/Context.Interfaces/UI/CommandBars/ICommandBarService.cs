using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Context.Interfaces.UI.CommandBars
{
    [Guid("D4B9569E-9E00-4910-846E-8F1BE5EAE41B")]
    public interface ICommandBarService
    {
        ICommandBar AddCommandBar(CommandBarType type, string name, string displayName, ICommandBarControl attachTo, int position, bool defaultVisible, Guid id);
        ICommandBarControl AddBarControl(string commandName, string commandGroup, string commandBarName, int position);
        ICommandBarControl AddBarComboBox(string commandName, string commandGroup, string commandBarName, int position, int width, bool showCaption);
        ICommandBarControl AddNotifyIconControl(string commandName, string commandGroup);

        void UpdateControls();

        bool GetUIContextActive(Guid contextId);
        void SetUIContextActive(Guid contextId, bool active);

        ICommandBar this[int index] { get; }
        ICommandBar this[string name] { get; }
        int Count { get; }
    }
}
