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
        private readonly string _connectionString;

        private SQLiteDataAccess db = new SQLiteDataAccess();

        public IList<LogModel> GetAllLogs()
        {
            string sql = "Select iD, Timestamp, Level, Exception, RenderedMessage, Properties from Logs";
            return this.db.LoadData<LogModel, dynamic>(sql, new { }, _connectionString);
        }

        public IList<LogModel> GetSomeLogs(int logCount)
        {
            string sql = $"SELECT * FROM Logs ORDER BY iD DESC LIMIT {logCount}";
            return this.db.LoadData<LogModel, dynamic>(sql, new { }, _connectionString);
        }

        public SQLiteCRUD(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
