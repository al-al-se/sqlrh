using System.Data;
using Npgsql;

public class PostgreSQLExecutor : IDBMSExecutor
{
    public DataTable ExecuteReader(string connectionString, string query)
    {
        //https://www.npgsql.org/doc/index.html
        //Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase
        using (var connection = new NpgsqlConnection(connectionString))
        {          
            connection.Open();
            
            var command = new NpgsqlCommand(query, connection);

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
        using (var connection = new NpgsqlConnection(connectionString))
        {          
            connection.Open();
            
            var command = new NpgsqlCommand(query, connection);

            return command.ExecuteScalar();
        }
    }
}
