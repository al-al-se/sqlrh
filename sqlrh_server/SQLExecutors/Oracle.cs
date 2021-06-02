using System.Data;
using Oracle.ManagedDataAccess.Client;

public class OracleExecutor : IDBMSExecutor
{
    public DataTable ExecuteReader(string connectionString, string query)
    {
        using (var connection = new OracleConnection(connectionString))
        {          
            connection.Open();
            
            var command = new OracleCommand(query, connection);

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
        using (var connection = new OracleConnection(connectionString))
        {          
            connection.Open();
            
            var command = new OracleCommand(query, connection);

            return command.ExecuteScalar();
        }
    }
}
