using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public interface IReportBuilderService
{
    string ReportTemplatePath {get; init;}

    string TempPath {get; init;}

    Task<string> SaveUploadingReportTemplate(IFormFile uploadingFile);

    string StartReportBuilding(string reportTemplatePath);

    bool CheckReportStartBuilding(string reportPath);

    bool CheckReportFinished(string reportPath);
}