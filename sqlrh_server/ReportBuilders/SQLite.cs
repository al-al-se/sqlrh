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

            var reader = command.ExecuteReader();

            DataTable dt = new DataTable();

            dt.Load(reader);    

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
