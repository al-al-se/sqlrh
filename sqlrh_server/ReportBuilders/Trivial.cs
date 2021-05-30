using System.IO;
using System.Threading.Tasks;
class TrivialReportBuilder : IReportBuilder
{
    public IExternalDataBaseRepository DataBases {get; set;}

    public void Build(string trmplatePth, string reportPath)
    {
                  using (Stream source = File.Open(templatePth, FileMode.Open))
            {
                using(Stream destination = File.Create(reportPath))
                {
                    source.CopyTo(destination);
                }
            }
    }

    public async Task BuildAsync(string templatePth, string reportPath)
    {
          using (Stream source = File.Open(templatePth, FileMode.Open))
            {
                using(Stream destination = File.Create(reportPath))
                {
                    await source.CopyToAsync(destination);
                }
            }
    }
}

