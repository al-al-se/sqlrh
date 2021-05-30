using System.Text;
using System.IO;
using System.Data;
using System.Collections.Generic;

class HtmlReportBuilder : TxtReportBuilder
{
    public HtmlReportBuilder(IExternalDataBaseRepository r) : base(r)
    {

    }
    public  override void Write(DataTable dt)
    {

    }
}