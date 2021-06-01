using System.IO;
using System.Threading.Tasks;
class TrivialReportBuilder : IReportBuilder
{
    public int DataBaseRepositotyTimeoutMilisec {get; set;}

    public IReportBuilder SetDataBaseRepositotyTimeoutMilisec(int value) {return this;}

    public void Build(string templatePath, string reportPath)
    {
                  using (Stream source = File.Open(templatePath, FileMode.Open))
            {
                using(Stream destination = File.Create(reportPath))
                {
                    source.CopyTo(destination);
                }
            }
    }

    public async Task BuildAsync(string templatePath, string reportPath)
    {
          using (Stream source = File.Open(templatePath, FileMode.Open))
            {
                using(Stream destination = File.Create(reportPath))
                {
                    await source.CopyToAsync(destination);
                }
            }
    }
}

