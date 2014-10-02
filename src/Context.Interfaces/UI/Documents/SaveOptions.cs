using System;

namespace Context.Interfaces.UI.Documents
{
    [Flags]
    public enum SaveOptions
    {
        Prompt = 1,
        NoSave = 2,
        ForceSave = 4,
        ForcePrompt = 8,
        ShowItemList = 0x100,
        NoCancelButton = 0x200,
    }
}
