using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISheduleRepository
{
    Task<IEnumerable<Shedule>> GetAll();

    Task<SqlrhUser> Add(Shedule s);

    Task<SqlrhUser> Change(Shedule s);

    Task Delete(int id);
}