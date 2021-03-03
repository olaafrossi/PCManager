using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace PCManager.DataAccess.Library
{
    public class SQLiteDataAccess
    {
        public List<T> LoadData<T, U>(string sqlStatement, U parameters, string connectionString)
        {
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(sqlStatement, parameters).ToList();
                return rows;
            }
        }
    }
}
