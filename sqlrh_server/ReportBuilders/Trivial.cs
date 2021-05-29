using System.IO;
using System.Threading.Tasks;
class TrivialReportBuilder : IReportBuilder
{
    public async Task Build(string templatePth, string reportPath)
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

