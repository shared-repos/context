using System;
using Context.Core;
using System.Diagnostics;

namespace Context.Service
{
    public class ProductService : ServiceRoot
    {
        public ProductService()
            : base(new Startup())
        {
        }
    }
}
