using System;
using System.Data;
using System.Data.Common;

public abstract class GenericExecutor<Connection,Command> : IDBMSExecutor
                where Connection : DbConnection, new()
                where Command : DbCommand, new()
{
    protected abstract Connection NewConnection(string connectionString);

    protected abstract Command NewCommand(string query, Connection c);
    public DataTable ExecuteReader(string connectionString, string query)
    {
        using (var connection = NewConnection(connectionString))
        {          
            connection.Open();
            
            var command = NewCommand(query, connection);

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
        using (var connection = NewConnection(connectionString))
        {          
            connection.Open();
            
            var command = NewCommand(query, connection);

            return command.ExecuteScalar();
        }
    }
}
