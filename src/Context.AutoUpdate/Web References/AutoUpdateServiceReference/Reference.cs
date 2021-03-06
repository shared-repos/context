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

namespace Context.AutoUpdate.AutoUpdateServiceReference {
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
    [System.Web.Services.WebServiceBindingAttribute(Name="AutoUpdateSoap", Namespace="http://tempuri.org/")]
    public partial class AutoUpdate : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetUpdatesOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public AutoUpdate() {
            this.Url = global::Context.AutoUpdate.Properties.Settings.Default.AutoUpdateService_AutoUpdate;
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
        public event GetUpdatesCompletedEventHandler GetUpdatesCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetUpdates", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public Package GetUpdates(SystemInfo systemInfo, ProductInfo productInfo) {
            object[] results = this.Invoke("GetUpdates", new object[] {
                        systemInfo,
                        productInfo});
            return ((Package)(results[0]));
        }
        
        /// <remarks/>
        public void GetUpdatesAsync(SystemInfo systemInfo, ProductInfo productInfo) {
            this.GetUpdatesAsync(systemInfo, productInfo, null);
        }
        
        /// <remarks/>
        public void GetUpdatesAsync(SystemInfo systemInfo, ProductInfo productInfo, object userState) {
            if ((this.GetUpdatesOperationCompleted == null)) {
                this.GetUpdatesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetUpdatesOperationCompleted);
            }
            this.InvokeAsync("GetUpdates", new object[] {
                        systemInfo,
                        productInfo}, this.GetUpdatesOperationCompleted, userState);
        }
        
        private void OnGetUpdatesOperationCompleted(object arg) {
            if ((this.GetUpdatesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetUpdatesCompleted(this, new GetUpdatesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    public partial class SystemInfo {
        
        private string systemNameField;
        
        private string systemVersionLabelField;
        
        private string componentInfosField;
        
        private string cultureNameField;
        
        /// <remarks/>
        public string SystemName {
            get {
                return this.systemNameField;
            }
            set {
                this.systemNameField = value;
            }
        }
        
        /// <remarks/>
        public string SystemVersionLabel {
            get {
                return this.systemVersionLabelField;
            }
            set {
                this.systemVersionLabelField = value;
            }
        }
        
        /// <remarks/>
        public string ComponentInfos {
            get {
                return this.componentInfosField;
            }
            set {
                this.componentInfosField = value;
            }
        }
        
        /// <remarks/>
        public string CultureName {
            get {
                return this.cultureNameField;
            }
            set {
                this.cultureNameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class PackageItem {
        
        private string downloadPathField;
        
        private string targetPathField;
        
        private string updatePathField;
        
        private byte[] assemblyPublicKeyField;
        
        private byte[] mD5HashField;
        
        private string fileVersionLabelField;
        
        private long fileSizeField;
        
        private bool isRestartRequiredField;
        
        /// <remarks/>
        public string DownloadPath {
            get {
                return this.downloadPathField;
            }
            set {
                this.downloadPathField = value;
            }
        }
        
        /// <remarks/>
        public string TargetPath {
            get {
                return this.targetPathField;
            }
            set {
                this.targetPathField = value;
            }
        }
        
        /// <remarks/>
        public string UpdatePath {
            get {
                return this.updatePathField;
            }
            set {
                this.updatePathField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] AssemblyPublicKey {
            get {
                return this.assemblyPublicKeyField;
            }
            set {
                this.assemblyPublicKeyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] MD5Hash {
            get {
                return this.mD5HashField;
            }
            set {
                this.mD5HashField = value;
            }
        }
        
        /// <remarks/>
        public string FileVersionLabel {
            get {
                return this.fileVersionLabelField;
            }
            set {
                this.fileVersionLabelField = value;
            }
        }
        
        /// <remarks/>
        public long FileSize {
            get {
                return this.fileSizeField;
            }
            set {
                this.fileSizeField = value;
            }
        }
        
        /// <remarks/>
        public bool IsRestartRequired {
            get {
                return this.isRestartRequiredField;
            }
            set {
                this.isRestartRequiredField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class Package {
        
        private string idField;
        
        private string titleField;
        
        private string categoryField;
        
        private string descriptionField;
        
        private string installScriptField;
        
        private string productVersionLabelField;
        
        private bool isRestartRequiredField;
        
        private bool isRestartUIRequiredField;
        
        private PackageItem[] itemsField;
        
        /// <remarks/>
        public string Id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public string Title {
            get {
                return this.titleField;
            }
            set {
                this.titleField = value;
            }
        }
        
        /// <remarks/>
        public string Category {
            get {
                return this.categoryField;
            }
            set {
                this.categoryField = value;
            }
        }
        
        /// <remarks/>
        public string Description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
        
        /// <remarks/>
        public string InstallScript {
            get {
                return this.installScriptField;
            }
            set {
                this.installScriptField = value;
            }
        }
        
        /// <remarks/>
        public string ProductVersionLabel {
            get {
                return this.productVersionLabelField;
            }
            set {
                this.productVersionLabelField = value;
            }
        }
        
        /// <remarks/>
        public bool IsRestartRequired {
            get {
                return this.isRestartRequiredField;
            }
            set {
                this.isRestartRequiredField = value;
            }
        }
        
        /// <remarks/>
        public bool IsRestartUIRequired {
            get {
                return this.isRestartUIRequiredField;
            }
            set {
                this.isRestartUIRequiredField = value;
            }
        }
        
        /// <remarks/>
        public PackageItem[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ProductInfo {
        
        private string productVersionLabelField;
        
        private string productIdField;
        
        private string buildTypeNameField;
        
        private string productNameField;
        
        private string editionField;
        
        private string cultureNameField;
        
        private string installedAtField;
        
        private string stateField;
        
        /// <remarks/>
        public string ProductVersionLabel {
            get {
                return this.productVersionLabelField;
            }
            set {
                this.productVersionLabelField = value;
            }
        }
        
        /// <remarks/>
        public string ProductId {
            get {
                return this.productIdField;
            }
            set {
                this.productIdField = value;
            }
        }
        
        /// <remarks/>
        public string BuildTypeName {
            get {
                return this.buildTypeNameField;
            }
            set {
                this.buildTypeNameField = value;
            }
        }
        
        /// <remarks/>
        public string ProductName {
            get {
                return this.productNameField;
            }
            set {
                this.productNameField = value;
            }
        }
        
        /// <remarks/>
        public string Edition {
            get {
                return this.editionField;
            }
            set {
                this.editionField = value;
            }
        }
        
        /// <remarks/>
        public string CultureName {
            get {
                return this.cultureNameField;
            }
            set {
                this.cultureNameField = value;
            }
        }
        
        /// <remarks/>
        public string InstalledAt {
            get {
                return this.installedAtField;
            }
            set {
                this.installedAtField = value;
            }
        }
        
        /// <remarks/>
        public string State {
            get {
                return this.stateField;
            }
            set {
                this.stateField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void GetUpdatesCompletedEventHandler(object sender, GetUpdatesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetUpdatesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetUpdatesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Package Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Package)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591