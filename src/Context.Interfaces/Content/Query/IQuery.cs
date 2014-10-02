using System;
using System.Collections.Generic;

namespace Context.Interfaces.Content.Query
{
    public interface IQuery
    {
        IContentType ContentType { get; }

        int TransactionId { get; }
    }
}
