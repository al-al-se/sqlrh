using System.Data;
using Oracle.ManagedDataAccess.Client;

public class OracleExecutor : GenericExecutor<OracleConnection,OracleCommand>
{
    protected override OracleConnection NewConnection(string connectionString)
    {
        return new OracleConnection(connectionString);
    }

    protected override OracleCommand NewCommand(string query, OracleConnection connection)
    {
        return new OracleCommand(query,connection);
    }
}
