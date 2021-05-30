using System.Collections.Generic;
using System.Threading.Tasks;

public interface IExternalDataBaseRepository
{
    Task<IEnumerable<ExternalDatabase>> GetAll();
    Task<string> GetConnectionString(string alias);

    
}