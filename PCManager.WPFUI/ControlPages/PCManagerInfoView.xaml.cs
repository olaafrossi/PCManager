// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 02 28
// by Olaaf Rossi

using System;
using System.Threading.Tasks;
using System.Windows;
using PCManager.DataAccess.Library;
using PCManager.WPFUI.Controllers;
using Serilog;
using ModernWpf.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GridView = ModernWpf.Controls.GridView;

namespace PCManager.WPFUI.ControlPages
{
    /// <summary>
    ///     Interaction logic for PCManagerInfoView.xaml
    /// </summary>
    public partial class PCManagerInfoView
    {
        private readonly PCManagerInfoController viewModel = new();

        public PCManagerInfoView()
        {
            this.Loaded += this.OnLoaded;
            this.InitializeComponent();
            Log.Logger.Information("PCManager Info View Started");
        }

        private static string GetConnectionString()
        {
            string output = string.Empty;
            output = Properties.Resources.ConnectionString;
            Console.WriteLine(output);
            return output;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.OnLoaded;
            this.DataContext = await this.viewModel.GetAppDataAsync().ConfigureAwait(true);
            await Task.Run(this.GetDataLogs).ConfigureAwait(true);
        }

        private void GetDataLogs()
        {
            SQLiteCRUD sql = new SQLiteCRUD(GetConnectionString());
            Log.Logger.Information("The Log Data Method");
            var rows = sql.GetSomeLogs(20);
            this.Dispatcher.Invoke(() => { this.LogGrid.ItemsSource = rows; });
        }
    }
}