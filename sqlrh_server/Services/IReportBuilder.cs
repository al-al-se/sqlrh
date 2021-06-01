using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public interface IReportBuilderService
{
    string ReportTemplatePath {get; init;}

    string TempPath {get; init;}

    IExternalDataBaseRepository DataBases {get; init;}

    Task<string> SaveUploadingReportTemplate(IFormFile uploadingFile);

    Task<string> StartReportBuilding(string reportTemplatePath, Action<string> onReportFinished = null);

    bool CheckReportStartBuilding(string reportPath);

    bool CheckReportFinished(string reportPath);
}