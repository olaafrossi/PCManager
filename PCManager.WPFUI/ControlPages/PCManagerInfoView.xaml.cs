// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 02 28
// by Olaaf Rossi

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

using PCManager.DataAccess.Library;
using PCManager.WPFUI.Controllers;

using Serilog;

namespace PCManager.WPFUI.ControlPages
{
    /// <summary>
    ///     Interaction logic for PCManagerInfoView.xaml
    /// </summary>
    public partial class PCManagerInfoView
    {
        private readonly Stopwatch stopwatch;

        private readonly PCManagerInfoController viewModel = new();

        public PCManagerInfoView()
        {
            this.stopwatch = Stopwatch.StartNew();
            this.Loaded += this.OnLoaded;
            this.InitializeComponent();
            Log.Logger.Information("PCManager Info View Started");
        }

        private static string GetConnectionString()
        {
            string output = string.Empty;
            output = Properties.Resources.ConnectionStringLogs;
            Log.Logger.Information("Getting SQL Connection String for LogDB {output}", output);
            return output;
        }

        private void CheckForUpdateOnClick(object sender, RoutedEventArgs e)
        {
            //TODO: implement
            Log.Logger.Information("Checking for MSIX application update{sender} {e}", sender, e);
        }

        private void GetDataLogs()
        {
            this.stopwatch.Restart();
            SQLiteCRUD sql = new SQLiteCRUD(GetConnectionString());
            string logComboBoxSelection = string.Empty;

            // get the number of logs from the user
            this.Dispatcher.Invoke(() => { return logComboBoxSelection = this.LogSelectComboBox.SelectionBoxItem.ToString(); });
            int numOfLogs = 20;
            try
            {
                numOfLogs = int.Parse(logComboBoxSelection);
                Log.Logger.Information("Getting Data Logs{numOfLogs}", numOfLogs);
            }
            catch (Exception e)
            {
                Log.Logger.Error("Didn't parse the number in the Log ComboBox (this is impossible) {numOfLogs}", numOfLogs);
            }

            var rows = sql.GetSomeLogs(numOfLogs);

            // insert the rows into the LogGrid
            this.Dispatcher.Invoke(() => { this.LogGrid.ItemsSource = rows; }, DispatcherPriority.DataBind);

            _ = this.Dispatcher.BeginInvoke(() => { this.LoadTimeTextBlock.Text = $" DB query time: {this.stopwatch.ElapsedMilliseconds} ms"; }, DispatcherPriority.DataBind);
            Log.Logger.Information("Inserted DB rows into the LogGrid in {this.stopwatch.ElapsedMilliseconds}", this.stopwatch.ElapsedMilliseconds);
            this.stopwatch.Stop();
        }

        private void LinkTo3ByteOnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Log.Logger.Information("User clicked {sender} {e}", sender, e);
            var destURL = "https://www.google.com/";
            var sInfo = new ProcessStartInfo(destURL) { UseShellExecute = true };
            Process.Start(sInfo);
        }

        private void LinkToGitHubProjectOnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Log.Logger.Information("User clicked {sender} {e}", sender, e);
            var destURL = "https://www.bing.com/";
            var sInfo = new ProcessStartInfo(destURL) { UseShellExecute = true };
            Process.Start(sInfo);
        }

        private void LinkToProjectInstallerOnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Log.Logger.Information("User clicked {sender} {e}", sender, e);
            var destURL = "https://www.bing.com/";
            var sInfo = new ProcessStartInfo(destURL) { UseShellExecute = true };
            Process.Start(sInfo);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.OnLoaded;
            this.DataContext = await this.viewModel.GetAppDataAsync().ConfigureAwait(true);
            await Task.Run(this.GetDataLogs).ConfigureAwait(true);
        }

        private void RefreshLogButtonOnClick(object sender, RoutedEventArgs e)
        {
            Log.Logger.Information("Clicked the Refresh Button");
            this.GetDataLogs();
        }
    }
}