using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAccessRepository
{
    Task<IEnumerable<AccessRule>> GetAll();
    Task Allow(int ReportId, int UserId);

    Task Disallow(int ReportId, int UserId);
}