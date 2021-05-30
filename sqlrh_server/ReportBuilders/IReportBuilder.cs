using System.Threading.Tasks;

public interface IReportBuilder
{
    IExternalDataBaseRepository DataBases {get; set;}

    void Build(string templatePth, string reportPath);
    Task BuildAsync(string templatePth, string reportPath);
}