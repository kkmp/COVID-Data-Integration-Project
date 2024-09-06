using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SoapClient.DataService;

namespace SoapClient.Models
{
    internal class WindowViewModel : INotifyPropertyChanged
    {
        private string confirmed;
        private string deaths;

        public DateTime? Date { get; set; }
        public ICommand DataCommand { get; set; }
        public string Confirmed { get => confirmed; 
            set { 
                confirmed = value;
                NotifyPropertyChanged();
            }
        }
        public string Deaths
        {
            get => deaths;
            set
            {
                deaths = value;
                NotifyPropertyChanged();
            }
        }
        public WindowViewModel()
        {
            DataCommand = new RelayCommand<int>( x => GetData(), x => Date != null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void GetData()
        {
            SOAPInterfaceClient soapInterfaceClient = new SOAPInterfaceClient();
            var response = soapInterfaceClient.getData(Date.Value.ToString("yyyy-MM-dd"));
            Confirmed = response[0].ToString();
            Deaths = response[1].ToString();
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
