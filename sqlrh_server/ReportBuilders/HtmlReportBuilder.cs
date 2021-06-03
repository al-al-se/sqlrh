using System.Text;
using System.IO;
using System.Data;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

class HtmlReportBuilder : TxtReportBuilder
{
    public HtmlReportBuilder(ISQLQueryExecutor  r, ILogger l) 
        : base(r,l)
    {

    }
    public  override void WriteTable(DataTable dt, string format)
    {

    }
}