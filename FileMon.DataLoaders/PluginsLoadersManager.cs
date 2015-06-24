using FileMon.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileMon.DataLoaders
{
    public class PluginsLoadersManager : IDataLoadersManager
    {
        private readonly ConcurrentDictionary<string, Type> _loaders;
        private readonly ILogger _logger;

        public List<string> SupportedExtensions { get; private set; }

        public PluginsLoadersManager(string pluginDirectory, IEnumerable<Tuple<string, string, string>> fileList, ILogger logger)
        {
            _loaders = new ConcurrentDictionary<string, Type>();
            SupportedExtensions = new List<string>();
            _logger = logger;

            string[] pluginFiles = Directory.GetFiles(pluginDirectory, "*.dll");
            foreach(var plugin in fileList)
            {
                var asm = Assembly.LoadFile(Path.Combine(pluginDirectory, plugin.Item3));
                var type = asm.GetExportedTypes().Where(t => plugin.Item2 == t.Name).FirstOrDefault();
                if (type == null)
                {
                    _logger.Warn(string.Format("Loader's type: {0} is not present in assembly: {1}", plugin.Item2, plugin.Item3));
                    continue;
                }
                    

                _loaders[plugin.Item1] = type;
                SupportedExtensions.Add(plugin.Item1);                
            }
        }

        public IDataLoader GetDataLoaderByFilter(string type)
        {
            if (!_loaders.ContainsKey(type))
            {
                _logger.Warn(string.Format("Filter type: {0} is not supported", type));
                return null;
            }
                

            var inst = Activator.CreateInstance(_loaders[type], _logger) as IDataLoader;
            if (inst == null)
            {
                _logger.Warn(string.Format("Instance of type: {0} could not be created", type));
                return null;
            }               

            return inst;
        }
    }
}
