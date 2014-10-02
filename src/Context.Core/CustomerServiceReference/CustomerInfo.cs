using System;
using System.Collections.Generic;
using System.Text;
using Context.Interfaces.Common;

namespace Context.Core.CustomerServiceReference
{
    public partial class CustomerInfo : ICustomerInfo
    {
        #region ICustomerInfo Members

        public Guid ID
        {
            get { return Guid.Empty; }
        }

        public DateTime Registered
        {
            get { return DateTime.MinValue; }
        }

        #endregion
    }
}
