using System.Threading.Tasks;

public interface IReportBuilder
{
    Task Build(string trmplatePth, string reportPath);
}