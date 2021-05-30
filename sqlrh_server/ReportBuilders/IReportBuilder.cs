using System.Threading.Tasks;

public interface IReportBuilder
{
    IExternalDataBaseRepository DataBases {get; set;}
    Task Build(string trmplatePth, string reportPath);
}