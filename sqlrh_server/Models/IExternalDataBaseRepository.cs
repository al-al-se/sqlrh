using System.Collections.Generic;
using System.Threading.Tasks;

public interface IExternalDataBaseRepository
{
    Task<IEnumerable<ExternalDatabase>> GetAll();
    Task<string> GetConnectionString(string alias); 

    Task<ExternalDatabase> Get(string alias);

    Task<ExternalDatabase> Add(string alias, string dbms, string connectionString);

    Task<bool> Contains(string alias);

    Task<ExternalDatabase> Change(string alias, string connectionString);

    Task Delete(string alias);
}