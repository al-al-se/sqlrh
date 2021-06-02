using System.Data;
using System.Data.SqlClient;

public class SQLServerExecutor : GenericExecutor<SqlConnection,SqlCommand>
{
    protected override SqlConnection NewConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }

    protected override SqlCommand NewCommand(string query, SqlConnection connection)
    {
        return new SqlCommand(query,connection);
    }
}
