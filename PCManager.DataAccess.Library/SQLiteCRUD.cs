using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCManager.DataAccess.Library.Models;

namespace PCManager.DataAccess.Library
{
    public class SQLiteCRUD
    {
        private readonly string connectionString;

        private SQLiteDataAccess db = new SQLiteDataAccess();

        public SQLiteCRUD(string connString)
        {
            this.connectionString = connString;
        }

        public IList<LogModel> GetAllLogs()
        {
            string sql = "Select iD, Timestamp, Level, Exception, RenderedMessage, Properties from Logs";
            return this.db.LoadData<LogModel, dynamic>(sql, new { }, this.connectionString);
        }

        public IList<LogModel> GetSomeLogs(int logCount)
        {
            string sql = $"SELECT * FROM Logs ORDER BY iD DESC LIMIT {logCount}";
            return this.db.LoadData<LogModel, dynamic>(sql, new { }, this.connectionString);
        }

        public IList<NetworkMessageModel> GetSomeNetData(int logCount)
        {
            string sql = $"SELECT * FROM Messages ORDER BY iD DESC LIMIT {logCount}";
            return this.db.LoadData<NetworkMessageModel, dynamic>(sql, new { }, this.connectionString);
        }

        public void InsertNetMessage(NetworkMessageModel msg)
        {
            string sql = "insert into Messages (Timestamp, UDPPort, RemoteIP, IncomingMessage, OutgoingMessage) values (@Timestamp, @UDPPort, @RemoteIP, @IncomingMessage, @OutgoingMessage);";
            this.db.SaveData(sql, new {msg.Timestamp, msg.UDPPort, msg.RemoteIP, msg.IncomingMessage, msg.OutgoingMessage}, this.connectionString);
        }
    }
}
