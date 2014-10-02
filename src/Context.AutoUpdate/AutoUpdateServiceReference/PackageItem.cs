using System;
using System.IO;
using Context.Interfaces.AutoUpdate;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Context.AutoUpdate.AutoUpdateServiceReference
{
    public partial class PackageItem : IPackageItem
    {
        #region IPackageItem Members

        [XmlIgnore]
        [JsonIgnore]
        public FileInfo TargetFileInfo
        {
            get { return new FileInfo(TargetPath); }
        }

        [XmlIgnore]
        [JsonIgnore]
        public FileInfo UpdateFileInfo
        {
            get { return new FileInfo(UpdatePath); }
        }

        [XmlIgnore]
        [JsonIgnore]
        public Version FileVersion
        {
            get { return new Version(FileVersionLabel); }
        }

        #endregion
    }
}
