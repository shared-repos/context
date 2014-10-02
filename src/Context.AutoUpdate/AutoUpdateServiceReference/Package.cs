using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Context.Interfaces.AutoUpdate;

namespace Context.AutoUpdate.AutoUpdateServiceReference
{
    public partial class Package : IPackage
    {
        #region IPackage Members

        [XmlIgnore]
        IList<IPackageItem> IPackage.Items
        {
            get
            {
                ReadOnlyCollection<IPackageItem> list = new ReadOnlyCollection<IPackageItem>(Items);
                return list;
            }
        }

        #endregion
    }
}
