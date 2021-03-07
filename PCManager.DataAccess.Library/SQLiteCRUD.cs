// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 03 06
// by Olaaf Rossi

using System.Collections.Generic;

using PCManager.DataAccess.Library.Models;

namespace PCManager.DataAccess.Library
{
    public class SQLiteCRUD
    {
        private readonly string connectionString;

        private readonly SQLiteDataAccess db = new();

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

        public IList<NetworkMessageModel> GetSomeNetData(int msgCount)
        {
            string sql = $"SELECT * FROM Network ORDER BY iD DESC LIMIT {msgCount}";
            return this.db.LoadData<NetworkMessageModel, dynamic>(sql, new { }, this.connectionString);
        }

        public void InsertNetMessage(NetworkMessageModel msg)
        {
            string sql =
                "insert into Network (Timestamp, UDPPort, RemoteIP, RemotePort, IncomingMessage, OutgoingMessage) values (@Timestamp, @UDPPort, @RemoteIP, @RemotePort, @IncomingMessage, @OutgoingMessage);";
            this.db.SaveData(
                sql,
                new
                    {
                        msg.Timestamp,
                        msg.UDPPort,
                        msg.RemoteIP,
                        msg.RemotePort,
                        msg.IncomingMessage,
                        msg.OutgoingMessage
                    },
                this.connectionString);
        }
    }
}