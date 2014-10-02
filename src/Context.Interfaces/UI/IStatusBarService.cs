using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Context.Interfaces.UI
{
    [Guid("CAF68206-391C-4413-9B9F-C2FF28BCA624")]
    public interface IStatusBarService
    {
        Bitmap Animation { get; set; }
        void SetText(string text);
        void SetText(string text, Color foreColor, Color backColor);
        void SetLineColumnChar(int line, int column, int character);
        void Progress(bool inProgress, string label, int complete, int total);
        void Clear();
    }
}
