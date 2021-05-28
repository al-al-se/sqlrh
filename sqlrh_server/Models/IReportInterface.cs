using System.Collections.Generic;
using System.Threading.Tasks;

public interface IReportRepository
{
    Task<IEnumerable<Report>> GetAll();

    Task<Report> Add(string name);

    Task<Report> LoadFile(int id, string path);
}