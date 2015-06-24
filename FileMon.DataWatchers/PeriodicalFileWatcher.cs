using FileMon.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileMon.DataWatchers
{
    public class PeriodicalFileWatcher : IDataWatcher
    {
        private readonly IDataProcessor<string> _dataProcessor;
        private readonly FileWatcherOptions _options;
        private readonly ConcurrentDictionary<string, bool> _processedFiles;
        private readonly CancellationTokenSource _ctSrc;
        private readonly ILogger _logger;

        public PeriodicalFileWatcher(FileWatcherOptions options, IDataProcessor<string> dataProcessor, ILogger logger)
        {
            _options = options;            
            _dataProcessor = dataProcessor;
            _processedFiles = new ConcurrentDictionary<string, bool>();
            _ctSrc = new CancellationTokenSource();
            _logger = logger;
        }

        public void SartWatch()
        {
            foreach(var filter in _options.Filters)
            {
                Task.Run(() =>
                {
                    while (!_ctSrc.IsCancellationRequested)
                    {
                        foreach (string file in Directory.GetFiles(_options.Path, filter))
                        {
                            if (_processedFiles.ContainsKey(file))
                                continue;
                            _dataProcessor.Process(file);
                            _processedFiles[file] = true;
                        }
                        if(_options.Frequency > 0)
                            Task.Delay(_options.Frequency).Wait();
                    }
                }, _ctSrc.Token)
                .ContinueWith((t) =>
                {
                    _logger.Error(t.Exception.Message);
                }, TaskContinuationOptions.OnlyOnFaulted);
            }
            
        }

        public void StopWatch()
        {
            _ctSrc.Cancel();
        }
    }
}
