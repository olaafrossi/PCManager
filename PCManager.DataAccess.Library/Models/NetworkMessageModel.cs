using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCManager.DataAccess.Library.Models
{
    public class NetworkMessageModel
    {
        public int iD { get; set; }

        public string Timestamp { get; set; }

        public int UDPPort { get; set; }

        public string RemoteIP { get; set; }

        public string IncomingMessage { get; set; }

        public string OutgoingMessage { get; set; }
    }
}
