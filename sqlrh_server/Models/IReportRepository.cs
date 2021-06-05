using System.Collections.Generic;
using System.Threading.Tasks;

public interface IReportRepository
{
    Task<IEnumerable<Report>> GetAll(string userLogin);

    Task<bool> IsReportAvailableToUser(int id, string login);

    Task<Report> Add(string name);

    Task<bool> ContainsId(int id);

    Task<Report> GetReport(int id);

    Task<Report> LoadFile(int id, string path);

    Task Delete(int id);

    Task<AccessRule> Allow(int id, string login);

    Task<AccessRule> Disallow(int id, string login);
}