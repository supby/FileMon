using FileMon.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileMon.UI
{
    public class MainWindowViewModel : IObserver<TradeDataEntity>, INotifyPropertyChanged
    {
        private ObservableCollection<TradeDataEntity> _data;
        public ObservableCollection<TradeDataEntity> Data 
        {
            get { return _data; }
        }

        public MainWindowViewModel() //TODO: inject logger here
        {
            _data = new ObservableCollection<TradeDataEntity>();
            _data.CollectionChanged += _data_CollectionChanged;
        }

        void _data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("Data"));
        }

        public void OnCompleted()
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("Data"));
        }

        private void MergeNewDataEntity(TradeDataEntity dataItem)
        {
            Action<TradeDataEntity> addMethod = _data.Add;
            Application.Current.Dispatcher.BeginInvoke(addMethod, dataItem);
        }

        public void OnError(Exception error)
        {
            //TODO: log error here
        }

        public void OnNext(TradeDataEntity value)
        {
            MergeNewDataEntity(value);     
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
    }
}
