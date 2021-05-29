using System.Collections.Generic;
using System.Threading.Tasks;

public interface IReportRepository
{
    Task<IEnumerable<Report>> GetAll();

    Task<Report> Add(string name);

    Task<bool> ContainsId(int id);

    Task<Report> GetReport(int id);

    Task<Report> LoadFile(int id, string path);
}