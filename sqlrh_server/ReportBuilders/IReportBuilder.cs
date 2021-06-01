using System.Threading.Tasks;

public interface IReportBuilder
{
    IExternalDataBaseRepository DataBases {get; set;}

    ISQLQueryExecutor SQLExecutor {get; set;}

    int DataBaseRepositotyTimeoutMilisec {get; set;}

    IReportBuilder SetDataBaseRepositotyTimeoutMilisec(int value);

    void Build(string templatePth, string reportPath);
    Task BuildAsync(string templatePth, string reportPath);
}