using System;
using Context.Interfaces.System;

namespace Context.Interfaces.UI.Documents
{
    public class HierarchyAsyncErrorEventArgs : AsyncExceptionEventArgs
    {
        private bool canContinue;
        private HierarchyErrorAction errorAction;
        private INode node;

        public HierarchyAsyncErrorEventArgs(IAsyncOperation asyncOperation, Exception exception, bool canContinue, INode node)
            : base(asyncOperation, exception)
        {
            this.canContinue = canContinue;
            this.node = node;
            this.errorAction = HierarchyErrorAction.Abort;
        }

        public bool CanContinue
        {
            get
            {
                return this.canContinue;
            }
        }

        public HierarchyErrorAction ErrorAction
        {
            get
            {
                return this.errorAction;
            }
            set
            {
                this.errorAction = value;
            }
        }

        public INode Node
        {
            get
            {
                return this.node;
            }
        }
    }
}
