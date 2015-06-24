using FileMon.Interfaces;
using FileMon.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileMon.Plugins
{
    public class XMLFileLoader : IDataLoader
    {
        private readonly ILogger _logger;

        public XMLFileLoader(ILogger logger)
        {
            _logger = logger;
        }

        public IObservable<T> LoadData<T>(string fileName)
        {
            Func<IObserver<T>, Task> s = obs =>
            {
                return Task.Run(() =>
                {
                    using (FileStream fileSteam = File.OpenRead(fileName))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.ConformanceLevel = ConformanceLevel.Document;

                        using (XmlReader reader = XmlReader.Create(fileSteam, settings))
                        {
                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element && reader.Name == "value")
                                {
                                    TradeDataEntity dataEntity = new TradeDataEntity();
                                    reader.MoveToAttribute("date");
                                    dataEntity.Date = DateTime.Parse(reader.Value);
                                    reader.MoveToAttribute("open");
                                    dataEntity.Open = Convert.ToDecimal(reader.Value);
                                    reader.MoveToAttribute("high");
                                    dataEntity.High = Convert.ToDecimal(reader.Value);
                                    reader.MoveToAttribute("low");
                                    dataEntity.Low = Convert.ToDecimal(reader.Value);
                                    reader.MoveToAttribute("close");
                                    dataEntity.Close = Convert.ToDecimal(reader.Value);
                                    reader.MoveToAttribute("volume");
                                    dataEntity.Volume = Convert.ToDecimal(reader.Value);

                                    obs.OnNext((T)Convert.ChangeType(dataEntity, typeof(T)));                                    
                                }
                                reader.MoveToElement();
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
