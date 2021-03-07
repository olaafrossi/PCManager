// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 03 06
// by Olaaf Rossi

using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

using Dapper;

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

        public void SaveData<T>(string sqlStatement, T parameters, string connectionString)
        {
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Execute(sqlStatement, parameters);
            }
        }
    }
}