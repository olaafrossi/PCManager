using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serilog;

namespace PCManager.WPFUI.Helpers
{
    public static class ConnectionStringHelper
    {
        public enum DataBases
        {
            Logs,
            Network,
        }

        public static string GetConnectionString(DataBases dB)
        {
            string output = string.Empty;

            if (dB == DataBases.Logs)
            {
                output = Properties.Resources.ConnectionStringLogs;
                Log.Logger.Information("Getting SQL Connection String for LogDB {output}", output);
            }
            else if (dB == DataBases.Network)
            {
                output = Properties.Resources.ConnectionStringNetwork;
                Log.Logger.Information("Getting SQL Connection String for NetworkDB {output}", output);
            }
            else
            {
                output = "Not a valid DB";
            }
            return output;
        }
    }
}
