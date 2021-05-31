using System;
using Xunit;
using Moq;
using System.Data;
using System.IO;

namespace sqlrh_server_tests
{
    public class TxtReportBuilderUnitTest
    {
        [Fact]
        public void WriteTableTest()
        {
            var dbMock = new Mock<IExternalDataBaseRepository>();

            var sqlMock = new Mock<ISQLQueryExecutor>();

            TxtReportBuilder tb = new TxtReportBuilder(dbMock.Object, sqlMock.Object);

            string srcFileName = Path.Combine(Directory.GetCurrentDirectory(),"src2.txt");
            
            using (var src_file = new StreamWriter(srcFileName)) 
            {

            }

            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn("col1 ", typeof(int)));
            dt.Columns.Add(new DataColumn("col2  ",typeof(float)));

            string format = ",-5:d;,7:f3";

            DataRow r = dt.NewRow();
            r[0] = 1;
            r[1] = 1.23456;
            dt.Rows.Add(r);

            r = dt.NewRow();
            r[0] = 2;
            r[1] = -21.23456;
            dt.Rows.Add(r);

            string obtained_result = "";

            // for editing copy to text editor and break lines after \n
            string expected_result = "------------------\n| col1  | col2   |\n------------------\n| 1     |  1,235 |\n------------------\n| 2     |-21,235 |\n------------------\n";

            using (var src = new StreamReader(srcFileName))
            {
                using (var dst = new StringWriter())
                {

                    tb.OpenFilesForMock(src,dst);

                    tb.WriteTable(dt,format);

                    obtained_result = dst.ToString();
                    Assert.Equal(expected_result,obtained_result);
                }
            }

            File.Delete(srcFileName);
        }
    }
}
