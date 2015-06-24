using FileMon.Interfaces;
using FileMon.Models;
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
    public class TextFileLoader : IDataLoader
    {
        private readonly ILogger _logger;

        public TextFileLoader(ILogger logger)
        {
            _logger = logger;
        }

        public IObservable<T> LoadData<T>(string fileName)
        {
            Func<IObserver<T>, Task> s = obs =>
            {                
                return Task.Run(() =>
                {
                    using(StreamReader sr = new StreamReader(fileName))
                    {
                        string line;
                        bool firstLine = true;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if(firstLine)
                            {
                                firstLine = false;
                                continue;
                            }

                            string[] items = line.Split(';');

                            TradeDataEntity dataEntity = new TradeDataEntity()
                            {
                                Date = DateTime.Parse(items[0]),
                                Open = Convert.ToDecimal(items[1]),
                                High = Convert.ToDecimal(items[2]),
                                Low = Convert.ToDecimal(items[3]),
                                Close = Convert.ToDecimal(items[4]),
                                Volume = Convert.ToDecimal(items[5])
                            };
                            obs.OnNext((T)Convert.ChangeType(dataEntity, typeof(T)));                            
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
