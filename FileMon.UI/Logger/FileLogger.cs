using FileMon.Interfaces;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMon.UI.Logger
{
    public class FileLogger : ILogger
    {        
        private NLog.Logger _logger;        

        public FileLogger()
        {            
            _logger = LogManager.GetLogger("FileLogger");
        }

        public void Info(string info)
        {
            Write(info, LogLevel.Info);
        }

        public void Warn(string warn)
        {
            Write(warn, LogLevel.Warn);
        }

        public void Error(string error)
        {
            Write(error, LogLevel.Error);
        }

        private void Write(string msg, LogLevel level)
        {
            _logger.Log(level, msg);
        }
    }
}
