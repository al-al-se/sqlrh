using System.Text;
using System.IO;
using System.Data;
using System.Collections.Generic;

class MdReportBuilder : TxtReportBuilder
{
    public MdReportBuilder(IExternalDataBaseRepository r) : base(r)
    {

    }
    public  override void Write(DataTable dt)
    {

    }
}