using System.Text;
using System.IO;
using System.Data;
using System.Collections.Generic;

class HtmlReportBuilder : TxtReportBuilder
{
    public HtmlReportBuilder(IExternalDataBaseRepository r, ISQLQueryExecutor e) 
        : base(r,e)
    {

    }
    public  override void WriteTable(DataTable dt, string format)
    {

    }
}