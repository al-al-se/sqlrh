using System.Threading.Tasks;

public interface IReportBuilder
{
    int DataBaseRepositotyTimeoutMilisec {get; set;}

    IReportBuilder SetDataBaseRepositotyTimeoutMilisec(int value);

    void Build(string templatePth, string reportPath);
    Task BuildAsync(string templatePth, string reportPath);
}