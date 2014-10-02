using System;
using System.Collections.Generic;

namespace Context.Interfaces.Content
{
    public interface IContentItem
    {
        IContentFile File { get; }

        object this[IContentMember member] { get; set; }

        IContentType ContentType { get; set; }
    }
}
