using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Context.Interfaces.Common
{
    [Guid("8B633CE5-36DE-43df-BF92-3C7341CD4088")]
    public interface ICustomerService
    {
        string CreateCustomer(ICustomerInfo customerInfo);

        ICustomerInfo GetCustomer(string customerTicket);

        string CustomerTicket { get; set; }

        void ReportError(string[] parts);
    }
}
