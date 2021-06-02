using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

class SQLQueryExecutor : ISQLQueryExecutor
{
    public SQLQueryExecutor(IEnumerable<ExternalDatabase> dbs)
    {
        DataBases = dbs;
    }
    public IEnumerable<ExternalDatabase> DataBases {get; init;}

    public DataTable ExecuteReader(QueryData q)
    {
        q.database = GetDB(q.alias);
        return GetSQLQueryExecutor(q).
            ExecuteReader(q.database.ConnectionString,q.sqlQuery);
    }
    public object ExecuteScalar(QueryData q)
    {
        q.database = GetDB(q.alias);
        return GetSQLQueryExecutor(q).
            ExecuteScalar(q.database.ConnectionString,q.sqlQuery);
    }

    public IDBMSExecutor GetSQLQueryExecutor(QueryData q)
    {
        switch (q.database.DBMS.ToLower())
        {
            case "sqlite":
                return new SQLiteExecutor();
            case "mysql":
                return new MySqlExecutor();
            case "sqlserver":
                return new SQLServerExecutor();
            case "orecle":
                return new OracleExecutor();
            case "postgresql":
                return new PostgreSQLExecutor();
            default:
                throw new KeyNotFoundException(
                    $"DBMS driver for {q.database.DBMS} not found");
        }
    }

    public ExternalDatabase GetDB(string alias)
    {
        try
        {
            return DataBases.First(i => i.Alias == alias);
        } catch (Exception e)
        {
            //log e
            throw new KeyNotFoundException(
                $"Database {alias} not found, exception : {e.Message} {e.Source} {e.StackTrace}");
        }
    }
}