using FileMon.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMon.Plugins
{
    public class CSVFileLoader : IDataLoader
    {
        private readonly ILogger _logger;

        public CSVFileLoader(ILogger logger)
        {
            _logger = logger;
        }

        public IObservable<T> LoadData<T>(string fileName)
        {
            Func<IObserver<T>, Task> s = obs =>
            {
                return Task.Run(() =>
                {
                    var csvConf = new CsvConfiguration();                    
                    csvConf.HasHeaderRecord = false;

                    using (var sr = File.OpenText(fileName))
                    {
                        using (var csv = new CsvReader(sr, csvConf))
                        {                            
                            while (csv.Read())
                            {
                                obs.OnNext(csv.GetRecord<T>());
                            }
                        }
                        _logger.Info(string.Format("File '{0}' has been processed", fileName)); 
                    }                    
                })
                .ContinueWith((t) =>
                {
                    _logger.Error(t.Exception.Message);
                }, TaskContinuationOptions.OnlyOnFaulted);
            };

            return Observable.Create<T>(s);
        }       
    }
}
