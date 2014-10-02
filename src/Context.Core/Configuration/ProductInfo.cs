using System;
using System.Collections.Generic;
using System.Text;
using Context.Interfaces.Common;
using Context.Interfaces.Services;

namespace Context.Core.Configuration
{
    internal class ProductInfo : IProductInfo
    {
        private const string ApplicationNameSetting = "ApplicationName";
        private const string DescriptionSetting = "Description";
        private const string ProductIdSetting = "ProductId";
        private const string ProductVersionSetting = "ProductVersion";
        private const string ProductTypeSetting = "ProductType";
        private const string BuildTypeSetting = "BuildType";
        private const string ApplicationIconSetting = "ApplicationIcon";
        private const string CultureNameSetting = "CultureName";
        private const string CompanyNameSetting = "CompanyName";
        private const string ProductNameSetting = "ProductName";
        private const string EditionSetting = "Edition";
        private const string RegistryRootSetting = "RegistryRoot";
        private const string OptionsRootSetting = "OptionsRoot";
        private const string ConfigFolderSetting = "ConfigFolder";
        private const string DataFolderSetting = "DataFolder";
        private const string CopyrightSetting = "Copyright";

        private readonly IPropertyProvider properties;

        public ProductInfo(IPropertyProvider properties)
        {
            this.properties = properties;
        }

        #region IProductInfo Members

        public string ApplicationName
        {
            get { return Convert.ToString(properties[ApplicationNameSetting]); }
        }

        public string Description
        {
            get { return Convert.ToString(properties[DescriptionSetting]); }
        }

        public string ProductId
        {
            get { return Convert.ToString(properties[ProductIdSetting]); }
        }

        public Version ProductVersion
        {
            get { return new Version(Convert.ToString(properties[ProductVersionSetting])); }
        }

        public ProductType ProductType
        {
            get { return (ProductType)Enum.Parse(typeof(ProductType), Convert.ToString(properties[ProductTypeSetting]), true); }
        }

        public BuildType BuildType
        {
            get { return (BuildType)Enum.Parse(typeof(BuildType), Convert.ToString(properties[BuildTypeSetting]), true); }
        }

        public System.Drawing.Icon ApplicationIcon
        {
            get { return new System.Drawing.Icon(Convert.ToString(properties[ApplicationIconSetting])); }
        }

        public string CultureName
        {
            get { return Convert.ToString(properties[CultureNameSetting]); }
        }

        public string CompanyName
        {
            get { return Convert.ToString(properties[CompanyNameSetting]); }
        }

        public string ProductName
        {
            get { return Convert.ToString(properties[ProductNameSetting]); }
        }

        public string Edition
        {
            get { return Convert.ToString(properties[EditionSetting]); }
        }

        public string RegistryRoot
        {
            get
            {
                var str = Convert.ToString(properties[RegistryRootSetting]);
                if (string.IsNullOrEmpty(str))
                {
                    return string.Format(@"Software\{0}\{1}", CompanyName, ApplicationName);
                }

                return str;
            }
        }

        public string OptionsRoot
        {
            get
            {
                var str = Convert.ToString(properties[OptionsRootSetting]);
                if (string.IsNullOrEmpty(str))
                {
                    return string.Format(@"Software\{0}\{1}", CompanyName, ProductName);
                }

                return str;
            }
        }

        public string ConfigFolder
        {
            get
            {
                var str = Convert.ToString(properties[ConfigFolderSetting]);
                if (string.IsNullOrEmpty(str))
                {
                    return System.IO.Path.Combine(this.CompanyName, this.ApplicationName);
                }

                return str;
            }
        }

        public string DataFolder
        {
            get { return Convert.ToString(properties[DataFolderSetting]); }
        }

        public string Copyright
        {
            get { return Convert.ToString(properties[CopyrightSetting]); }
        }

        public string GetInfoString(string type)
        {
            return Convert.ToString(properties[type]);
        }

        #endregion
    }
}
