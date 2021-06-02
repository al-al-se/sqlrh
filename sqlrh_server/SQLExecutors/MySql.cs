using System.Data;
using MySql.Data.MySqlClient;

public class MySqlExecutor : GenericExecutor<MySqlConnection,MySqlCommand>
{
    protected override MySqlConnection NewConnection(string connectionString)
    {
        return new MySqlConnection(connectionString);
    }

    protected override MySqlCommand NewCommand(string query, MySqlConnection connection)
    {
        return new MySqlCommand(query,connection);
    }
}
