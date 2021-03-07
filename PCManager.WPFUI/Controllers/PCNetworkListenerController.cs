// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 03 06
// by Olaaf Rossi

using System;

using PCManager.DataAccess.Library;
using PCManager.DataAccess.Library.Models;
using PCManager.WPFUI.Helpers;

using ThreeByteLibrary.Dotnet;

namespace PCManager.WPFUI.Controllers
{
    public class PCNetworkListenerController
    {
        public event EventHandler<int> UDPPortSet;
        
        public PCNetworkListenerController(NetworkMessagesEventArgs incomingMsg)
        {
            SQLiteCRUD sql = new SQLiteCRUD(ConnectionStringHelper.GetConnectionString(ConnectionStringHelper.DataBases.Network));
            NetworkMessageModel netMsg = new NetworkMessageModel();

            if (incomingMsg.IncomingMessage is null)
            {
                netMsg.IncomingMessage = string.Empty;
            }
            else
            {
                netMsg.IncomingMessage = incomingMsg.IncomingMessage;
            }

            if (incomingMsg.OutgoingMessage is null)
            {
                netMsg.IncomingMessage = string.Empty;
            }
            else
            {
                netMsg.OutgoingMessage = incomingMsg.OutgoingMessage;
            }

            if (incomingMsg.RemoteIP is null)
            {
                incomingMsg.RemoteIP = string.Empty;
            }
            else
            {
                netMsg.RemoteIP = incomingMsg.RemoteIP;
            }

            if (incomingMsg.IncomingMessage is null)
            {
                incomingMsg.Timestamp = string.Empty;
            }
            else
            {
                netMsg.Timestamp = incomingMsg.Timestamp;
            }

            if (incomingMsg.RemotePort is null)
            {
                incomingMsg.RemotePort = string.Empty;
            }
            else
            {
                netMsg.RemotePort = incomingMsg.RemotePort;
            }

            netMsg.UDPPort = incomingMsg.UDPPort;

            this.UDPPortSet?.Invoke(this, incomingMsg.UDPPort);

            sql.InsertNetMessage(netMsg);
        }
    }
}