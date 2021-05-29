using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

class ReportBuilderService : IReportBuilderService
{
    protected readonly ReportBuilderOptions _options;

    public string ReportTemplatePath {get; init;}

    protected string PrepareReportTemplatePath()
    {
        if (!string.IsNullOrEmpty(_options.FileStoragePath))
        {
            if (Directory.Exists(_options.FileStoragePath))
            {
                return  _options.FileStoragePath;
            }
        }

        return Path.Combine(Directory.GetCurrentDirectory(),"ReportTemplates");
    }

    public string TempPath {get; init;}

    protected string PrepareTempPath()
    {
        if (!string.IsNullOrEmpty(_options.FileStoragePath))
        {
            if (Directory.Exists(_options.FileStoragePath))
            {
                return  _options.FileStoragePath;
            }
        }

        return Path.Combine(Directory.GetCurrentDirectory(),"ReportTemp");
    }

    public ReportBuilderService(IOptions<ReportBuilderOptions> options)
    {
        _options = options.Value;

        ReportTemplatePath = PrepareReportTemplatePath();

        TempPath = PrepareTempPath();
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

    protected IReportBuilder GetBuilder(string ext)
    {
        switch(ext)
        {
            default: return new TrivialReportBuilder();
        }
    }

    public string StartReportBuilding(string reportTemplatePath)
    {
        string name = Path.GetFileName(reportTemplatePath);
        string ext = Path.GetExtension(reportTemplatePath);
        string reportName = $"{name}_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}{ext}";
        string fullReportName = Path.Combine(TempPath,reportName);
        string fullTempName = GetTemp(fullReportName);

        var t = GetBuilder(ext).Build(reportTemplatePath,fullTempName);
        t.ContinueWith(tr => File.Move(fullTempName,fullReportName));
        t.Start();

        return fullReportName;
    }

    protected string GetTemp(string f)
    {
        return $"{f}.tmp";
    }

    public bool CheckReportStartBuilding(string reportPath)
    {
        return File.Exists(GetTemp(reportPath));
    }

    public bool CheckReportFinished(string reportPath)
    {
        return File.Exists(reportPath);
    }
}