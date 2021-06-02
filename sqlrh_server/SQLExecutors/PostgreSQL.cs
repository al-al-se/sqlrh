using System.Data;
using Npgsql;

public class PostgreSQLExecutor : GenericExecutor<NpgsqlConnection,NpgsqlCommand>
{
    protected override NpgsqlConnection NewConnection(string connectionString)
    {
        //https://www.npgsql.org/doc/index.html
        //Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase
        return new NpgsqlConnection(connectionString);
    }

    protected override NpgsqlCommand NewCommand(string query, NpgsqlConnection connection)
    {
        return new NpgsqlCommand(query,connection);
    }  
}
