using System;
using System.Collections.Generic;
using System.Drawing;

namespace Context.Interfaces.UI.CommandBars
{
    public interface ICommandBar
    {
        event EventHandler Popup;

        ICommandBarControl AddControl(CommandBarControlType controlType, bool beginGroup, int imageId, int position);
        void RemoveControl(ICommandBarControl control);

        void Delete();
        void ShowPopup(Point position, ICommandTarget primaryCommandTarget);
        void UpdateControls();

        Guid Id { get; }
        string Name { get; }
        string DisplayName { get; }
        CommandBarType Type { get; }
        ICommandBarControl ParentControl { get; }
        bool Visible { get; }
        Visibility Visibility { get; set; }
        ICommandBarControl DefaultControl { get; set; }
        IList<ICommandBarControl> Controls { get; }
    }
}
