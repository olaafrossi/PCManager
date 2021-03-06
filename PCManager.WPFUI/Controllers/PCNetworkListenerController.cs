using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCManager.WPFUI.Controllers
{
    public class PCNetworkListenerController : ObservableCollection<PCNetworkListenerController.NetworkInputs>
    {

        public PCNetworkListenerController() : base()
        {
            Add(new NetworkInputs("Hello", DateTime.Now));
            Add(new NetworkInputs("Hello", DateTime.Now));
        }

        public class NetworkInputs : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string inputFrame = String.Empty;

            private DateTime dateTime;

            // This method is called by the Set accessor of each property.  
            // The CallerMemberName attribute that is applied to the optional propertyName  
            // parameter causes the property name of the caller to be substituted as an argument.  
            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public NetworkInputs(string input, DateTime time)
            {
                this.inputFrame = input;
                this.dateTime = time;
                NotifyPropertyChanged();
            }

            public string InputFrame
            {
                get
                {
                    return this.inputFrame;
                }
                set
                {
                    this.inputFrame = value;
                    NotifyPropertyChanged();
                }
            }

            public DateTime DateTime
            {
                get
                {
                    return this.dateTime;
                }
                set
                {
                    this.dateTime = value;
                }
            }
        }
    }
}
