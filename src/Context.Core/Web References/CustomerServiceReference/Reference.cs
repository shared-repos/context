﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.18444.
// 
#pragma warning disable 1591

namespace Context.Core.CustomerServiceReference {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="CustomerSoap", Namespace="http://tempuri.org/")]
    public partial class Customer : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback CreateCustomerWithInfoOperationCompleted;
        
        private System.Threading.SendOrPostCallback CreateCustomerOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetCustomerInfoOperationCompleted;
        
        private System.Threading.SendOrPostCallback ReportErrorOperationCompleted;
        
        private System.Threading.SendOrPostCallback ReportErrorDataOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Customer() {
            this.Url = global::Context.Core.Properties.Settings.Default.CustomerServiceReference_Customer;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event CreateCustomerWithInfoCompletedEventHandler CreateCustomerWithInfoCompleted;
        
        /// <remarks/>
        public event CreateCustomerCompletedEventHandler CreateCustomerCompleted;
        
        /// <remarks/>
        public event GetCustomerInfoCompletedEventHandler GetCustomerInfoCompleted;
        
        /// <remarks/>
        public event ReportErrorCompletedEventHandler ReportErrorCompleted;
        
        /// <remarks/>
        public event ReportErrorDataCompletedEventHandler ReportErrorDataCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CreateCustomerWithInfo", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string CreateCustomerWithInfo(CustomerInfo customerInfo) {
            object[] results = this.Invoke("CreateCustomerWithInfo", new object[] {
                        customerInfo});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void CreateCustomerWithInfoAsync(CustomerInfo customerInfo) {
            this.CreateCustomerWithInfoAsync(customerInfo, null);
        }
        
        /// <remarks/>
        public void CreateCustomerWithInfoAsync(CustomerInfo customerInfo, object userState) {
            if ((this.CreateCustomerWithInfoOperationCompleted == null)) {
                this.CreateCustomerWithInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCreateCustomerWithInfoOperationCompleted);
            }
            this.InvokeAsync("CreateCustomerWithInfo", new object[] {
                        customerInfo}, this.CreateCustomerWithInfoOperationCompleted, userState);
        }
        
        private void OnCreateCustomerWithInfoOperationCompleted(object arg) {
            if ((this.CreateCustomerWithInfoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CreateCustomerWithInfoCompleted(this, new CreateCustomerWithInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CreateCustomer", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string CreateCustomer(string loginName, string name) {
            object[] results = this.Invoke("CreateCustomer", new object[] {
                        loginName,
                        name});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void CreateCustomerAsync(string loginName, string name) {
            this.CreateCustomerAsync(loginName, name, null);
        }
        
        /// <remarks/>
        public void CreateCustomerAsync(string loginName, string name, object userState) {
            if ((this.CreateCustomerOperationCompleted == null)) {
                this.CreateCustomerOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCreateCustomerOperationCompleted);
            }
            this.InvokeAsync("CreateCustomer", new object[] {
                        loginName,
                        name}, this.CreateCustomerOperationCompleted, userState);
        }
        
        private void OnCreateCustomerOperationCompleted(object arg) {
            if ((this.CreateCustomerCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CreateCustomerCompleted(this, new CreateCustomerCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetCustomerInfo", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public CustomerInfo GetCustomerInfo(string customerTicket) {
            object[] results = this.Invoke("GetCustomerInfo", new object[] {
                        customerTicket});
            return ((CustomerInfo)(results[0]));
        }
        
        /// <remarks/>
        public void GetCustomerInfoAsync(string customerTicket) {
            this.GetCustomerInfoAsync(customerTicket, null);
        }
        
        /// <remarks/>
        public void GetCustomerInfoAsync(string customerTicket, object userState) {
            if ((this.GetCustomerInfoOperationCompleted == null)) {
                this.GetCustomerInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetCustomerInfoOperationCompleted);
            }
            this.InvokeAsync("GetCustomerInfo", new object[] {
                        customerTicket}, this.GetCustomerInfoOperationCompleted, userState);
        }
        
        private void OnGetCustomerInfoOperationCompleted(object arg) {
            if ((this.GetCustomerInfoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetCustomerInfoCompleted(this, new GetCustomerInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ReportError", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void ReportError(string[] parts) {
            this.Invoke("ReportError", new object[] {
                        parts});
        }
        
        /// <remarks/>
        public void ReportErrorAsync(string[] parts) {
            this.ReportErrorAsync(parts, null);
        }
        
        /// <remarks/>
        public void ReportErrorAsync(string[] parts, object userState) {
            if ((this.ReportErrorOperationCompleted == null)) {
                this.ReportErrorOperationCompleted = new System.Threading.SendOrPostCallback(this.OnReportErrorOperationCompleted);
            }
            this.InvokeAsync("ReportError", new object[] {
                        parts}, this.ReportErrorOperationCompleted, userState);
        }
        
        private void OnReportErrorOperationCompleted(object arg) {
            if ((this.ReportErrorCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ReportErrorCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ReportErrorData", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void ReportErrorData([System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] data) {
            this.Invoke("ReportErrorData", new object[] {
                        data});
        }
        
        /// <remarks/>
        public void ReportErrorDataAsync(byte[] data) {
            this.ReportErrorDataAsync(data, null);
        }
        
        /// <remarks/>
        public void ReportErrorDataAsync(byte[] data, object userState) {
            if ((this.ReportErrorDataOperationCompleted == null)) {
                this.ReportErrorDataOperationCompleted = new System.Threading.SendOrPostCallback(this.OnReportErrorDataOperationCompleted);
            }
            this.InvokeAsync("ReportErrorData", new object[] {
                        data}, this.ReportErrorDataOperationCompleted, userState);
        }
        
        private void OnReportErrorDataOperationCompleted(object arg) {
            if ((this.ReportErrorDataCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ReportErrorDataCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class CustomerInfo {
        
        private string loginNameField;
        
        private string nameField;
        
        /// <remarks/>
        public string LoginName {
            get {
                return this.loginNameField;
            }
            set {
                this.loginNameField = value;
            }
        }
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void CreateCustomerWithInfoCompletedEventHandler(object sender, CreateCustomerWithInfoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CreateCustomerWithInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CreateCustomerWithInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void CreateCustomerCompletedEventHandler(object sender, CreateCustomerCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CreateCustomerCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CreateCustomerCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void GetCustomerInfoCompletedEventHandler(object sender, GetCustomerInfoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetCustomerInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetCustomerInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public CustomerInfo Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((CustomerInfo)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void ReportErrorCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void ReportErrorDataCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
}

#pragma warning restore 1591