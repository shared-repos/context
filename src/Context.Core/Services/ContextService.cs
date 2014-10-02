using System;
using Context.Interfaces.Services;
using Context.Interfaces.Configuration;

namespace Context.Core
{
    internal class ContextService : IContextService
    {
        private readonly IServiceManager manager;
        private readonly IConfigurationManager configuration;

        [ThreadStatic]
        private static ContextScope currentScope;

        internal ContextService(IServiceManager manager)
        {
            this.manager = manager;
            this.configuration = (IConfigurationManager)manager.GetService(typeof(IConfigurationManager));
        }

        internal static Context FindContext(string scopeName)
        {
            ContextScope scope = currentScope;
            while (scope != null)
            {
                if (string.Compare(scope.Name, scopeName) == 0)
                {
                    return scope.Context;
                }

                scope = scope.Parent;
            }

            return null;
        }

        internal static void RemoveScope(ContextScope scope)
        {
            currentScope = scope.Parent;
        }

        #region IContextService Members

        public IContext Current
        {
            get
            {
                if (currentScope == null)
                {
                    return null;
                }

                return currentScope.Context;
            }
        }

        public IContext GetContext(string scopeName)
        {
            Context context = FindContext(scopeName);
            if (context != null)
            {
                return context;
            }

            throw new InvalidOperationException(string.Format("Context out of scope: {0}", scopeName));
        }

        public IDisposable CreateScope(string scopeName)
        {
            currentScope = new ContextScope(currentScope, CreateContext(scopeName), scopeName);
            return currentScope;
        }

        private Context CreateContext(string scopeName)
        {
            IPropertyProvider provider = configuration.GetSection(scopeName);
            Context parentContext = currentScope == null ? null : currentScope.Context;
            return new Context(scopeName, parentContext, manager, provider);
        }

        #endregion
    }
}
