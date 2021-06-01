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
        if (!string.IsNullOrEmpty(_options.TempPath))
        {
            if (Directory.Exists(_options.TempPath))
            {
                return  _options.TempPath;
            }
        }

        return Path.Combine(Directory.GetCurrentDirectory(),"ReportTemp");
    }

    public IExternalDataBaseRepository DataBases {get; init;}

    public ReportBuilderService(IOptions<ReportBuilderOptions> options,
                IExternalDataBaseRepository dbs)
    {
        _options = options.Value;

        ReportTemplatePath = PrepareReportTemplatePath();

        TempPath = PrepareTempPath();

        DataBases = dbs;
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

    protected async Task<IReportBuilder> GetBuilder(string ext)
    {
        var dbs = await DataBases.GetAll();
        switch(ext)
        {
            case ".txt": return new TxtReportBuilder(dbs);
            case ".md": return new MdReportBuilder(dbs);
            case ".html": return new HtmlReportBuilder(dbs);
            default: return new TrivialReportBuilder();
        }
    }

    public async Task<string> StartReportBuilding(string reportTemplatePath, Action<string> onReportFinished = null)
    {
        string name = Path.GetFileNameWithoutExtension(reportTemplatePath);
        string ext = Path.GetExtension(reportTemplatePath);
        string reportName = $"{name}_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}{ext}";
        string fullReportName = Path.Combine(TempPath,reportName);
        string fullTempName = GetTemp(fullReportName);

        var task = (await GetBuilder(ext)).
            SetDataBaseRepositotyTimeoutMilisec(_options.DataBaseRepositotyTimeoutMilisec).
            BuildAsync(reportTemplatePath,fullTempName).
            ContinueWith(tr => File.Move(fullTempName,fullReportName));

        if (onReportFinished != null) 
        {
            await task.ContinueWith(tr => onReportFinished(fullReportName));
        }
         
        return fullReportName;
    }

    public static string GetTemp(string f)
    {
        return $"{f}.tmp";
    }
    

    public bool CheckReportStartBuilding(string reportPath)
    {
        return File.Exists(GetTemp(reportPath)) || File.Exists(reportPath);
    }

    public bool CheckReportFinished(string reportPath)
    {
        return File.Exists(reportPath);
    }
}