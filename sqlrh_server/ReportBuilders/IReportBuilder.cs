using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public interface IReportBuilder
{
    int DataBaseRepositotyTimeoutMilisec {get; set;}

    IReportBuilder SetDataBaseRepositotyTimeoutMilisec(int value);

    ILogger Logger {get; set;}

    IReportBuilder SetLogger(ILogger l);

    void Build(string templatePth, string reportPath);
    Task BuildAsync(string templatePth, string reportPath);
}