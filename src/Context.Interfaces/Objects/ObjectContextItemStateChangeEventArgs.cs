using System;
using System.Collections;

namespace Context.Interfaces.Objects
{

    public sealed class ObjectContextItemStateChangeEventArgs : ObjectContextChangeEventArgs
    {
        private readonly ObjectState originalState;
        private readonly ObjectState currentState;

        public ObjectContextItemStateChangeEventArgs(IObjectContext context, object item, ObjectState originalState, ObjectState currentState)
            : base(context, item)
        {
            this.originalState = originalState;
            this.currentState = currentState;
        }

        public ObjectState OriginalState
        {
            get
            {
                return originalState;
            }
        }

        public ObjectState CurrentState
        {
            get
            {
                return currentState;
            }
        }
    }
}
