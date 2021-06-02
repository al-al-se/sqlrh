using System.Text;
using System.IO;
using System.Data;
using System.Collections.Generic;

class MdReportBuilder : TxtReportBuilder
{
    public MdReportBuilder(ISQLQueryExecutor  r) 
        : base(r)
    {

    }
    public  override void WriteTable(DataTable dt, string format)
    {

    }
}