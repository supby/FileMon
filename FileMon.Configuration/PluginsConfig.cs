using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMon.Configuration
{
    public class PluginConfig : ConfigurationElement
    {
        [ConfigurationProperty("filter", IsRequired = true)]
        public string ExtType
        {
            get
            {
                return this["filter"] as string;
            }
        }
        [ConfigurationProperty("classname", IsRequired = true)]
        public string ClassName
        {
            get
            {
                return this["classname"] as string;
            }
        }
        [ConfigurationProperty("filename", IsRequired = true)]
        public string FileName
        {
            get
            {
                return this["filename"] as string;
            }
        }
    }

    public class Plugins : ConfigurationElementCollection
    {
        public PluginConfig this[int index]
        {
            get
            {
                return base.BaseGet(index) as PluginConfig;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new PluginConfig this[string responseString]
        {
            get { return (PluginConfig)BaseGet(responseString); }
            set
            {
                if (BaseGet(responseString) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(responseString)));
                }
                BaseAdd(value);
            }
        }

        protected override System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new PluginConfig();
        }

        protected override object GetElementKey(System.Configuration.ConfigurationElement element)
        {
            return ((PluginConfig)element).ExtType;
        }
    }

    public class FileLoaderConfigurationSection : ConfigurationSection
    {

        public static FileLoaderConfigurationSection GetConfig()
        {
            return (FileLoaderConfigurationSection)System.Configuration.ConfigurationManager.GetSection("FileLoaderConfigurationSection") 
                                                                                                        ?? new FileLoaderConfigurationSection();
        }

        [System.Configuration.ConfigurationProperty("LoadersPlugins")]
        [ConfigurationCollection(typeof(Plugins), AddItemName = "Plugin")]
        public Plugins Plugins
        {
            get
            {
                object o = this["LoadersPlugins"];
                return o as Plugins;
            }
        }

    }
}
