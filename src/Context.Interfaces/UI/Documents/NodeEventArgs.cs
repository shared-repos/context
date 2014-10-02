using System;

namespace Context.Interfaces.UI.Documents
{
    public class NodeEventArgs : EventArgs
    {
        private IHierarchy hierarchy;
        private INode node;

        public NodeEventArgs(IHierarchy hierarchy, INode node)
        {
            this.hierarchy = hierarchy;
            this.node = node;
        }

        public IHierarchy Hierarchy
        {
            get
            {
                return this.hierarchy;
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
