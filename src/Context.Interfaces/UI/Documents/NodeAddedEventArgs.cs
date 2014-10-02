using System;

namespace Context.Interfaces.UI.Documents
{
    public class NodeAddedEventArgs : EventArgs
    {
        private IHierarchy hierarchy;
        private INode newNode;
        private INode parentNode;
        private INode previousSiblingNode;

        public NodeAddedEventArgs(IHierarchy hierarchy, INode parentNode, INode previousSiblingNode, INode newNode)
        {
            this.hierarchy = hierarchy;
            this.parentNode = parentNode;
            this.previousSiblingNode = previousSiblingNode;
            this.newNode = newNode;
        }

        public IHierarchy Hierarchy
        {
            get
            {
                return this.hierarchy;
            }
        }

        public INode NewNode
        {
            get
            {
                return this.newNode;
            }
        }

        public INode ParentNode
        {
            get
            {
                return this.parentNode;
            }
        }

        public INode PreviousSiblingNode
        {
            get
            {
                return this.previousSiblingNode;
            }
        }
    }
}
