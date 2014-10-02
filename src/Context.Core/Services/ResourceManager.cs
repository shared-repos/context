using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Globalization;
using Context.Interfaces.Services;

namespace Context.Core
{
    internal class ResourceManager : IResourceManager
    {
        private IModuleManager modules;
        private List<ImageListInfo> images;
        private List<ResourceInfo> resources;

        public ResourceManager(IServiceManager manager)
        {
            this.modules = (IModuleManager)manager.GetService(typeof(IModuleManager));
            this.images = new List<ImageListInfo>();
            this.resources = new List<ResourceInfo>();
            LoadResourceInfos();
        }

        private void LoadResourceInfos()
        {
            foreach (IModuleInfo module in modules.GetModuleList())
            {
                string images = Convert.ToString(module["Images"]);
                string resources = Convert.ToString(module["Resources"]);
                if (!string.IsNullOrEmpty(images))
                {
                    Dictionary<string, string> imageMap = StringHelpers.ParseDictionary(images, "-", ",", ";");
                    foreach (KeyValuePair<string, string> entry in imageMap)
                    {
                        this.images.Add(new ImageListInfo(int.Parse(entry.Key), int.Parse(entry.Value), module));
                    }
                }
                if (!string.IsNullOrEmpty(resources))
                {
                    string[] resourceMasks = resources.Split(',', ';');
                    foreach (string resourceMask in resourceMasks)
                    {
                        this.resources.Add(new ResourceInfo(resourceMask, module));
                    }
                }
            }
        }

        #region IResourceManager Members

        public Image GetImage(int imageId)
        {
            foreach (ImageListInfo info in images)
            {
                if (info.From <= imageId && imageId <= info.To)
                {
                    IModule module = modules.LoadModule(info.Module.Id);
                    IResourceManager manager = module as IResourceManager;
                    if (manager == null)
                    {
                        manager = (IResourceManager)module.GetService(typeof(IResourceManager));
                    }
                    if (manager != null)
                    {
                        return manager.GetImage(imageId);
                    }
                }
            }

            return null;
        }

        public string GetString(string name, CultureInfo culture)
        {
            IResourceManager manager = FindManager(name);
            if (manager != null)
            {
                return manager.GetString(name, culture);
            }
            return null;
        }

        public Stream GetStream(string name, CultureInfo culture)
        {
            IResourceManager manager = FindManager(name);
            if (manager != null)
            {
                return manager.GetStream(name, culture);
            }
            return null;
        }

        private IResourceManager FindManager(string name)
        {
            foreach (ResourceInfo info in resources)
            {
                if (Match(name, info.Mask))
                {
                    IModule module = modules.LoadModule(info.Module.Id);
                    IResourceManager manager = module as IResourceManager;
                    if (manager == null)
                    {
                        manager = (IResourceManager)module.GetService(typeof(IResourceManager));
                    }
                    return manager;
                }
            }

            return null;
        }

        private bool Match(string name, string mask)
        {
            if (mask.EndsWith("*"))
            {
                mask = mask.Remove(mask.Length - 1);
                return name.StartsWith(mask);
            }

            return name == mask;
        }

        #endregion

        private class ImageListInfo
        {
            private int from;
            private int to;
            private IModuleInfo module;

            public ImageListInfo(int from, int to, IModuleInfo module)
            {
                this.from = from;
                this.to = to;
                this.module = module;
            }

            public int From
            {
                get
                {
                    return from;
                }
            }

            public int To
            {
                get
                {
                    return to;
                }
            }

            public IModuleInfo Module
            {
                get
                {
                    return module;
                }
            }
        }

        private class ResourceInfo
        {
            private string mask;
            private IModuleInfo module;

            public ResourceInfo(string mask, IModuleInfo module)
            {
                this.mask = mask;
                this.module = module;
            }

            public string Mask
            {
                get
                {
                    return mask;
                }
            }

            public IModuleInfo Module
            {
                get
                {
                    return module;
                }
            }
        }
    }
}
