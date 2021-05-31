using System.Data;
using System.Data.Odbc;
public class SQLQueryExecutor : ISQLQueryExecutor
{
    public DataTable ExecuteReader(string connectionString, string query)
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

    public object ExecuteScalar(string connectionString, string query)
    {
        using (OdbcConnection connection = new OdbcConnection(connectionString))
        {
                connection.Open();
                
                OdbcCommand command = new OdbcCommand(query, connection);;

                return command.ExecuteScalar();
        }
    }
}
