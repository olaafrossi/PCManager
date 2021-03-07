// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 03 06
// by Olaaf Rossi

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using PCManager.DataAccess.Library;
using PCManager.DataAccess.Library.Models;
using PCManager.WPFUI.Helpers;

using Serilog;

using ThreeByteLibrary.Dotnet;

namespace PCManager.WPFUI.ControlPages
{
    /// <summary>
    ///     Interaction logic for PCNetworkListenerView.xaml
    /// </summary>
    public partial class PCNetworkListenerView
    {
        private readonly Stopwatch stopwatch;

        public PCNetworkListenerView()
        {
            this.stopwatch = Stopwatch.StartNew();
            this.InitializeComponent();
            this.GetNetworkData();
        }

        private void GetNetworkData()
        {
            Log.Logger.Information("Attempting to get SQL Connection");
            SQLiteCRUD sql = new SQLiteCRUD(ConnectionStringHelper.GetConnectionString(ConnectionStringHelper.DataBases.Network));

            string netComboBoxSelection = String.Empty;

            // get the number of logs from the user
            this.Dispatcher.Invoke(() => { return netComboBoxSelection = this.NetSelectComboBox.SelectionBoxItem.ToString(); });
            int numOfNetMsgs = 20;

            try
            {
                numOfNetMsgs = int.Parse(netComboBoxSelection);
                Log.Logger.Information("Getting Data Logs{numOfNetMsgs}", numOfNetMsgs);
            }
            catch (Exception e)
            {
                Log.Logger.Error("Didn't parse the number in the Log ComboBox (this is impossible) {numOfNetMsgs}", numOfNetMsgs);
            }

            var rows = sql.GetSomeNetData(numOfNetMsgs);

            // insert the rows into the NetworkGrid
            this.Dispatcher.Invoke(() => { this.NetworkGrid.ItemsSource = rows; }, DispatcherPriority.DataBind);

            _ = this.Dispatcher.BeginInvoke(() => { this.LoadTimeTextBlock.Text = $" DB query time: {this.stopwatch.ElapsedMilliseconds} ms"; }, DispatcherPriority.DataBind);
            Log.Logger.Information("Inserted DB rows into the NetGrid in {this.stopwatch.ElapsedMilliseconds}", this.stopwatch.ElapsedMilliseconds);
            this.stopwatch.Stop();
        }

        //public void SetNetworkData()
        //{
        //    SQLiteCRUD sql = new SQLiteCRUD(ConnectionStringHelper.GetConnectionString(ConnectionStringHelper.DataBases.Network));

        //    NetworkMessageModel netMsg = new NetworkMessageModel();
        //    netMsg.IncomingMessage = "Hello";
        //    netMsg.OutgoingMessage = "Boo!";
        //    netMsg.RemoteIP = "127.2.2.2";
        //    netMsg.Timestamp = DateTime.Now.ToLongTimeString();
        //    netMsg.UDPPort = 16009;

        //    sql.InsertNetMessage(netMsg);
        //}

        private void RefreshLogButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.GetNetworkData();
        }
    }
}