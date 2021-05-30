using System.Text;
using System.IO;
using System.Data;
using System.Collections.Generic;

class TxtReportBuilder : AbstractReportBuilder
{
    StreamReader source;
    StreamWriter destination;

    public TxtReportBuilder(IExternalDataBaseRepository r) : base(r)
    {

    }

    protected override void OpenFiles(string templatePth, string reportPath)
    {
        source = new StreamReader(templatePth);
        destination = new StreamWriter(reportPath);
    }

    protected override void CloseFiles()
    {
        source.Dispose();
        destination.Dispose();
    }

    public override bool EndOfSource() => source.EndOfStream;

    public override string ReadLine()=> source.ReadLine();

    public override void OnLineParsed()
                 => destination.Write(System.Environment.NewLine);
    public override void Write(string s) => destination.Write(s);
    public  override void Write(DataTable dt)
    {
        List<string> lines = new List<string>();

        StringBuilder sb = new StringBuilder();

    }
}