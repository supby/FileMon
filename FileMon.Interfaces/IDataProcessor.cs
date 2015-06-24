using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMon.Interfaces
{
    public interface IDataProcessor<T>
    {
        Task Process(T data);        
    }
}
