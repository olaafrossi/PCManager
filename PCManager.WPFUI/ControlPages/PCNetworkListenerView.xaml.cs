using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PCManager.WPFUI.Controllers;

using ThreeByteLibrary.Dotnet;

namespace PCManager.WPFUI.ControlPages
{
    /// <summary>
    /// Interaction logic for PCNetworkListenerView.xaml
    /// </summary>
    public partial class PCNetworkListenerView
    {
        private readonly PCNetworkListenerController viewModel = new();
        public PCNetworkListenerView()
        {
            //this.Loaded += this.OnLoaded;
            InitializeComponent();
            //UDPListBox.Document.Blocks.Add(new Paragraph(new Run("Whaa from view class")));
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.OnLoaded;

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            PCNetworkListenerController.NetworkInputs input = new PCNetworkListenerController.NetworkInputs("Happy happy", DateTime.Now);
        }
    }
}