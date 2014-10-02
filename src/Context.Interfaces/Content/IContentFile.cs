using System;
using System.Collections.Generic;
using System.IO;

namespace Context.Interfaces.Content
{
    public interface IContentFile
    {
        IContentItem Item { get; }

        string FileName { get; }

        int Length { get; }

        bool Exists { get; }

        Stream OpenStream();
    }
}
