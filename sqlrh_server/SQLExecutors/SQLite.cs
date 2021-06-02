using System.Data;
using Microsoft.Data.Sqlite;

public class SQLiteExecutor : GenericExecutor<SqliteConnection,SqliteCommand>
{
    protected override SqliteConnection NewConnection(string connectionString)
    {
        return new SqliteConnection(connectionString);
    }

    protected override SqliteCommand NewCommand(string query, SqliteConnection connection)
    {
        return new SqliteCommand(query,connection);
    }
}
