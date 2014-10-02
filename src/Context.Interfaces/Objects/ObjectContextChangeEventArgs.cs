using System;
using System.Collections;

namespace Context.Interfaces.Objects
{

    public class ObjectContextChangeEventArgs : EventArgs
    {
        private readonly IObjectContext context;
        private readonly object item;

        public ObjectContextChangeEventArgs(IObjectContext context, object item)
        {
            this.context = context;
            this.item = item;
        }

        public IObjectContext Context
        {
            get
            {
                return context;
            }
        }

        public object Item
        {
            get
            {
                return item;
            }
        }
    }
}
