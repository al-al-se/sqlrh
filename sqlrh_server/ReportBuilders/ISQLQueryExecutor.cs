using System.Data;

public interface ISQLQueryExecutor
{
    DataTable ExecuteReader(QueryData q);
    object ExecuteScalar(QueryData q);
}

public interface IDBMSExecutor
{
    DataTable ExecuteReader(string connectionString, string query);
    object ExecuteScalar(string connectionString, string query);
}