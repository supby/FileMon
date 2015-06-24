using FileMon.Configuration;
using FileMon.DataLoaders;
using FileMon.DataProcessors;
using FileMon.DataWatchers;
using FileMon.UI.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileMon.UI
{
    public static class Bootstrapper
    {
        public static void Initialise(MainWindowViewModel model)
        {
            // bootstrap dependenties
            // TODO: use IoC container for that
            var logger = new FileLogger();

            // load plugins
            FileLoaderConfigurationSection config = FileLoaderConfigurationSection.GetConfig();
            var plugins = new List<Tuple<string, string, string>>();
            foreach (PluginConfig pluginConf in config.Plugins)
            {
                plugins.Add(new Tuple<string, string, string>(pluginConf.ExtType, pluginConf.ClassName, pluginConf.FileName));
            }
            var plm = new PluginsLoadersManager(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), plugins, logger);            

            // bootstrap dependenties
            // TODO: use IoC container for that            
            var fp = new FileProcessor(plm, model);            
            new PeriodicalFileWatcher(new FileWatcherOptions(ConfigurationManager.AppSettings["DirectoryToMonitor"],
                                                        plm.SupportedExtensions, 
                                                        Convert.ToInt32(ConfigurationManager.AppSettings["DirectoryMonitorFrequencyMs"])),
                                      fp, logger).SartWatch();
        }
    }
}
