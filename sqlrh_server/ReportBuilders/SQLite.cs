using System.Data;
using Microsoft.Data.Sqlite;

public class SQLiteExecutor : ISQLQueryExecutor
{
    public DataTable ExecuteReader(string connectionString, string query)
    {
        using (var connection = new SqliteConnection(connectionString))
        {          
            connection.Open();
            
            var command = new SqliteCommand(query, connection);

            DataTable dt = new DataTable();
            using (var reader = command.ExecuteReader())
            {
                dt.Load(reader);    
            }
            return dt;            
        }
    }

    public object ExecuteScalar(string connectionString, string query)
    {
        using (var connection = new SqliteConnection(connectionString))
        {          
            connection.Open();
            
            var command = new SqliteCommand(query, connection);

            return command.ExecuteScalar();
        }
    }
}
