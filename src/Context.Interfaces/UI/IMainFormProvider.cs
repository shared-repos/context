using System;
using System.Runtime.InteropServices;

namespace Context.Interfaces.UI
{
    [Guid("9DE69F93-50F6-47a1-B4FC-99880324E59D")]
    public interface IMainFormProvider
    {
        object MainForm { get; }
    }
}
