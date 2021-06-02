using System.Data;
using System.Data.SqlClient;

public class SQLServerExecutor : IDBMSExecutor
{
    public DataTable ExecuteReader(string connectionString, string query)
    {
        using (var connection = new SqlConnection(connectionString))
        {          
            connection.Open();
            
            var command = new SqlCommand(query, connection);

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
        using (var connection = new SqlConnection(connectionString))
        {          
            connection.Open();
            
            var command = new SqlCommand(query, connection);

            return command.ExecuteScalar();
        }
    }
}
