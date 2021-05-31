using System.Data;

public interface ISQLQueryExecutor
{
    DataTable ExecuteReader(string connectionString, string query);
    object ExecuteScalar(string connectionString, string query);
}