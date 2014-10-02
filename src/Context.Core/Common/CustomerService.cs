using System;
using System.Collections.Generic;
using System.Text;
using Context.Interfaces.Common;
using Context.Interfaces.Services;
using Context.Core.CustomerServiceReference;
using System.Net;
using Newtonsoft.Json;

namespace Context.Core
{
    internal class CustomerService : ICustomerService
    {
        public const string CustomerServiceScope = "CustomerService";

        private const string ServiceUrlSettings = "ServiceUrl";
        private const string CustomerTicketSettings = "CustomerTicket";

        private readonly string serviceUrl;
        private readonly IContextService contextService;
        private readonly Customer service;
        private string customerTicket;
        private bool customerTicketSet;

        public CustomerService(IContextService contextService)
        {
            this.contextService = contextService;
            serviceUrl = Convert.ToString(contextService.Current[ServiceUrlSettings]);

            service = new Customer();
            service.Url = serviceUrl;
            service.CookieContainer = new CookieContainer();
        }

        #region ICustomerService Members

        public void ReportError(string[] parts)
        {
            service.CookieContainer.Add(CreateCustomerCookie(CustomerTicket));

            byte[] errorData = GZipStreamHelper.CompressString(JsonConvert.SerializeObject(parts));

            service.ReportErrorData(errorData);
        }

        public string CreateCustomer(ICustomerInfo customerInfo)
        {
            return service.CreateCustomer(customerInfo.LoginName, customerInfo.Name);
        }

        public ICustomerInfo GetCustomer(string customerTicket)
        {
            return service.GetCustomerInfo(customerTicket);
        }

        public string CustomerTicket
        {
            get
            {
                if (customerTicketSet)
                {
                    return customerTicket;
                }

                using (contextService.CreateScope(CustomerServiceScope))
                {
                    return Convert.ToString(contextService.Current[CustomerTicketSettings]);
                }
            }
            set
            {
                customerTicket = value;
                customerTicketSet = true;
            }
        }

        #endregion

        private Cookie CreateCustomerCookie(string customerTicket)
        {
            Cookie cookie = new Cookie(".CustomerTicket", customerTicket);
            cookie.Expires = DateTime.Now.AddDays(1000);
            cookie.Expired = false;
            cookie.Path = "/";
            Uri uri = new Uri(serviceUrl);
            cookie.Domain = uri.Authority;
            return cookie;
        }

    }
}
