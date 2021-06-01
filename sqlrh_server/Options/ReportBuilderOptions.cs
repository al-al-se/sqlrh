public class ReportBuilderOptions
{
    public const string SectionName = "ReportBuilder";

    public string FileStoragePath { get; set; }

    public string TempPath { get; set; }

    public int DataBaseRepositotyTimeoutMilisec {get; set;} = 2000;
}
