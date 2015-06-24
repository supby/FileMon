using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMon.Interfaces
{
    public interface ILogger
    {
        void Info(string info);
        void Warn(string warn);
        void Error(string error);
    }
}
