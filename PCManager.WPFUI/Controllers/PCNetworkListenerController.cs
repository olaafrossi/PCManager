using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using PCManager.DataAccess.Library;
using PCManager.DataAccess.Library.Models;

using Serilog;

using ThreeByteLibrary.Dotnet;

namespace PCManager.WPFUI.Controllers
{
    public class PCNetworkListenerController
    {
        private readonly Stopwatch stopwatch;

        public PCNetworkListenerController()
        {
            this.stopwatch = Stopwatch.StartNew();
        }

        public void SvcPcNetworkListener_MessageHit(object sender, PcNetworkListener.PCNetworkListenerMessages e)
        {
            Console.WriteLine($"Here I have a message???this is it: {sender}{e}");
            this.SetNetworkData();
        }

        private static string GetConnectionString()
        {
            string output = string.Empty;
            output = Properties.Resources.ConnectionStringNetwork;
            Log.Logger.Information("Getting SQL Connection String for NetworkDB {output}", output);
            return output;
        }

        public void SetNetworkData()
        {
            SQLiteCRUD sql = new SQLiteCRUD(GetConnectionString());

            NetworkMessageModel netMsg = new NetworkMessageModel();
            netMsg.IncomingMessage = "Hello";
            netMsg.OutgoingMessage = "Boo!";
            netMsg.RemoteIP = "1.1.1.1";
            netMsg.Timestamp = DateTime.Now.ToLongTimeString();
            netMsg.UDPPort = 1633009;

            sql.InsertNetMessage(netMsg);
            this.stopwatch.Stop();
        }
    }
}

