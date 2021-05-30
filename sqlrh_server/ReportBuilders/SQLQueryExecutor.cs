using System.Data;
using System.Data.Odbc;
public class SQLQueryExecutor
{
    public static DataTable ExecuteReader(string connectionString, string query)
    {
        using (OdbcConnection connection = new OdbcConnection(connectionString))
        {
            
                connection.Open();
                
                OdbcCommand command = new OdbcCommand(query, connection);

                OdbcDataReader reader = command.ExecuteReader();

                DataTable dt = new DataTable();

                dt.Load(reader);    

                return dt;            
        }
    }

    public static object ExecuteScalar(string connectionString, string query)
    {
        using (OdbcConnection connection = new OdbcConnection(connectionString))
        {
                connection.Open();
                
                OdbcCommand command = new OdbcCommand(query, connection);;

                return command.ExecuteScalar();
        }
    }
}
