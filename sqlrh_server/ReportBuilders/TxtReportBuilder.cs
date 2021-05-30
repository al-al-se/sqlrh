using System.Text.RegularExpressions;
using System;
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
    
    public  override void WriteTable(DataTable dt, string format)
    {
        List<string> lines = new List<string>();

        StringBuilder sb = new StringBuilder();

        sb.Append("|");
        foreach (DataColumn c in dt.Columns)
        {
            sb.Append($" {c.ColumnName} |");
        }

        int Width = sb.ToString().Length;
        string rowDelim = new string('_',Width);

        destination.WriteLine(rowDelim);
        destination.WriteLine(sb.ToString());
        destination.WriteLine(rowDelim);

        string[] formats = format.Split(',');
        foreach (DataRow r in dt.Rows)
        {
            Write("| ");
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                WriteScalar(r[i],formats[i]);
                Write(" |");
            }
            Write(System.Environment.NewLine);
            destination.WriteLine(rowDelim);
        }

    }
}