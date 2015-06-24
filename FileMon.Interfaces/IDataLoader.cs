using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMon.Interfaces
{
    public interface IDataLoader
    {        
        IObservable<T> LoadData<T>(string source);
    }
}
