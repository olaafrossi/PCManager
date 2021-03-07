// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 03 06
// by Olaaf Rossi

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using PCManager.DataAccess.Library;
using PCManager.DataAccess.Library.Models;
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

        private static string GetConnectionString()
        {
            string output = string.Empty;
            output = Properties.Resources.ConnectionStringNetwork;
            Log.Logger.Information("Getting SQL Connection String for NetworkDB {output}", output);
            return output;
        }

        private void GetNetworkData()
        {
            SQLiteCRUD sql = new SQLiteCRUD(GetConnectionString());
            var rows = sql.GetSomeNetData(20);

            // insert the rows into the NetworkGrid
            this.Dispatcher.Invoke(() => { this.NetworkGrid.ItemsSource = rows; }, DispatcherPriority.DataBind);
            this.stopwatch.Stop();
        }

        public void SetNetworkData()
        {
            SQLiteCRUD sql = new SQLiteCRUD(GetConnectionString());

            NetworkMessageModel netMsg = new NetworkMessageModel();
            netMsg.IncomingMessage = "Hello";
            netMsg.OutgoingMessage = "Boo!";
            netMsg.RemoteIP = "127.2.2.2";
            netMsg.Timestamp = DateTime.Now.ToLongTimeString();
            netMsg.UDPPort = 16009;

            sql.InsertNetMessage(netMsg);
        }

        private void RefreshLogButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.SetNetworkData();
            this.GetNetworkData();
        }
    }
}