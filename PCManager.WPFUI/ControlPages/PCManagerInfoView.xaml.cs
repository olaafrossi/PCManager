// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 02 28
// by Olaaf Rossi

using System.Windows;

using PCManager.WPFUI.Controllers;

namespace PCManager.WPFUI.ControlPages
{
    /// <summary>
    ///     Interaction logic for PCManagerInfoView.xaml
    /// </summary>
    public partial class PCManagerInfoView
    {
        private readonly PCManagerInfoController viewModel = new ();

        public PCManagerInfoView()
        {
            this.Loaded += this.OnLoaded;
            this.InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.OnLoaded;
            this.DataContext = await this.viewModel.GetDataAsync();
        }
    }
}