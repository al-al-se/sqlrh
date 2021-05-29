using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public interface IReportBuilder
{
    string ReportTemplatePath {get; init;}

    Task<string> SaveUploadingReportTemplate(IFormFile uploadingFile);
}