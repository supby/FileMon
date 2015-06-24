using FileMon.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMon.DataWatchers
{
    public class FileWatcherOptions
    {
        public FileWatcherOptions(string path, IEnumerable<string> filters, int frequency=0)
        {
            Path = path;
            Filters = filters;
            Frequency = frequency;
        }

        public string Path { get; private set; }
        public IEnumerable<string> Filters { get; private set; }
        public int Frequency { get; private set; }
    }

    public class FileWatcher : IDataWatcher
    {        
        private readonly IDataProcessor<string> _dataProcessor;
        private readonly FileWatcherOptions _options;
        private readonly List<FileSystemWatcher> _watchers;

        public FileWatcher(FileWatcherOptions options, IDataProcessor<string> dataProcessor)
        {
            _options = options;            
            _dataProcessor = dataProcessor;
            _watchers = new List<FileSystemWatcher>();
        }

        public void SartWatch()
        {
            foreach(string filter in _options.Filters)
            {
                var watcher = new FileSystemWatcher();
                watcher.Path = _options.Path;
                watcher.Filter = filter;
                watcher.NotifyFilter = NotifyFilters.FileName;
                watcher.Created += new FileSystemEventHandler(OnFileCreated);
                watcher.EnableRaisingEvents = true;
                lock(_watchers)
                {
                    _watchers.Add(watcher);
                }                
            }            
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            _dataProcessor.Process(e.FullPath);
        }


        public void StopWatch()
        {
            _watchers.ForEach(w => w.EnableRaisingEvents = false);
        }
    }
}
