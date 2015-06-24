using FileMon.Interfaces;
using FileMon.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMon.DataProcessors
{
    public class FileProcessor : IDataProcessor<string>
    {
        private readonly IDataLoadersManager _loadersManager;
        private readonly IObserver<TradeDataEntity> _observer;

        public FileProcessor(IDataLoadersManager loadersManager, IObserver<TradeDataEntity> observer)
        {
            _loadersManager = loadersManager;
            _observer = observer;
        }

        public Task Process(string dataFilename)
        {
            return Task.FromResult(_loadersManager
                    .GetDataLoaderByFilter(string.Format("*{0}", Path.GetExtension(dataFilename)))
                    .LoadData<TradeDataEntity>(dataFilename)
                    .Subscribe(_observer));
        }
    }
}
