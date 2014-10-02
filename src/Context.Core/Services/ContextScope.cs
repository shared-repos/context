using System;

namespace Context.Core
{
    internal class ContextScope : IDisposable
    {
        private readonly ContextScope parent;
        private readonly Context context;
        private readonly string name;

        public ContextScope(ContextScope parent, Context context, string name)
        {
            this.parent = parent;
            this.context = context;
            this.name = name;
        }

        public Context Context
        {
            get
            {
                return context;
            }
        }

        public ContextScope Parent
        {
            get
            {
                return parent;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            ContextService.RemoveScope(this);
        }

        #endregion
    }
}
