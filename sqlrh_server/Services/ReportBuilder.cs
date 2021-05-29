using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

class ReportBuilder : IReportBuilder
{
    protected readonly ReportBuilderOptions _options;

    public string ReportTemplatePath {get; init;}

    public ReportBuilder(IOptions<ReportBuilderOptions> options)
    {
        _options = options.Value;

        ReportTemplatePath = Path.Combine(Directory.GetCurrentDirectory(),"ReportTemplates");
        
        if (!string.IsNullOrEmpty(_options.FileStoragePath))
        {
            if (Directory.Exists(_options.FileStoragePath))
            {
                ReportTemplatePath = _options.FileStoragePath;
            }
        }
    } 
    public async Task<string> SaveUploadingReportTemplate(IFormFile uploadingFile)
    {
        var path = Path.Combine(ReportTemplatePath,uploadingFile.FileName);
            
        using (var fileStream = 
            new FileStream(path, FileMode.Create))
        {
            await  uploadingFile.CopyToAsync(fileStream);
        }

        return path;
    }
}