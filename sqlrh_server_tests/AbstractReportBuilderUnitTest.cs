using System.Text;
using System.IO;
using System;
using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace sqlrh_server_tests
{
    public class AbstractReportBuilderUnitTest
    {
        List<ExternalDatabase> dbs = new List<ExternalDatabase>{
                new ExternalDatabase()
                {
                    Alias = "1",
                    DBMS = "2",
                    ConnectionString = "3"
                }
            };

        Mock<ISQLQueryExecutor> sqlMock = new Mock<ISQLQueryExecutor>();

        public AbstractReportBuilderUnitTest()
        {

        }

        [Fact]
        public void WriteScalarTest()
        {
            TxtReportBuilder tb = new TxtReportBuilder(sqlMock.Object);

            string srcFileName = Path.Combine(Directory.GetCurrentDirectory(),"src.txt");
            
            using (var src_file = new StreamWriter(srcFileName)) 
            {

            }

            using (var src = new StreamReader(srcFileName))
            {
                using (var dst = new StringWriter())
                {
                    tb.OpenFilesForMock(src,dst);

                    int val = 12;
                    tb.WriteScalar(val,":d");

                    string obtained_result = dst.ToString();
                    string expected_result = "12";
                    Assert.Equal(obtained_result,expected_result);
                }

                using (var dst = new StringWriter())
                {
                    tb.OpenFilesForMock(src,dst);

                    int val = 12;
                    tb.WriteScalar(val,",-5:d");

                    string obtained_result = dst.ToString();
                    string expected_result = "12   ";
                    Assert.Equal(obtained_result,expected_result);
                }

                using (var dst = new StringWriter())
                {
                    tb.OpenFilesForMock(src,dst);

                    double val = 13.23789;
                    tb.WriteScalar(val,",5:f2");

                    string obtained_result = dst.ToString();
                    string expected_result = "13,24";
                    Assert.Equal(expected_result,obtained_result);
                }
            }

            File.Delete(srcFileName);

        }

        [Fact]
        public void ExecuteQueryTest()
        {
            string query = "s{,4:f1} db1 select min(value) from table_a";           

            object value = 1.23456789;

            sqlMock.Setup(a => a.ExecuteScalar(It.IsNotNull<QueryData>())).
                Returns(value);

            string expected_result = " 1,2";

            TxtReportBuilder tb = new TxtReportBuilder(sqlMock.Object);

            string srcFileName = Path.Combine(Directory.GetCurrentDirectory(),"src3.txt");
            
            using (var src_file = new StreamWriter(srcFileName)) 
            {

            }

            using (var src = new StreamReader(srcFileName))
            {
                using (var dst = new StringWriter())
                {
                    tb.OpenFilesForMock(src,dst);

                    tb.ParseQueryParametersAndExecute(query);

                    string obtained_result = dst.ToString();
                    
                    Assert.Equal(expected_result,obtained_result);
                }
            }

            File.Delete(srcFileName);
        }

        [Fact]
        public void FindSingleLineQueryTest()
        {
            string query = "s{,4:f1} db1 select min(value) from table_a where id > 2";

            object value = 1.23456789;

            string expected_result = " 1,2";

            sqlMock.Setup(a => a.ExecuteScalar(It.IsNotNull<QueryData>())).
                Returns(value);

            TxtReportBuilder tb = new TxtReportBuilder(sqlMock.Object);

            StringBuilder reportTemplate = new StringBuilder();
            StringBuilder expectedReport = new StringBuilder();
            reportTemplate.Append("aas d < g a<<1 sql ");
            expectedReport.Append("aas d < g a<<1 sql ");
            reportTemplate.Append(tb.OpeningTag());
            reportTemplate.Append(tb.Delim());
            reportTemplate.Append(query);
            reportTemplate.Append(tb.Delim());
            reportTemplate.Append(tb.ClosingTag());
            expectedReport.Append(expected_result);
            reportTemplate.Append(" fdvnkk  kfkdk k kkkf ");
            expectedReport.Append(" fdvnkk  kfkdk k kkkf ");

            string srcFileName = Path.Combine(Directory.GetCurrentDirectory(),"src4.txt");
            
            using (var src_file = new StreamWriter(srcFileName)) 
            {
                src_file.WriteLine(reportTemplate.ToString());
            }

            string dstFileName = Path.Combine(Directory.GetCurrentDirectory(),"src5.txt");

            tb.Build(srcFileName,dstFileName);

            Assert.Equal(query,tb.QueryText.ToString());

            string writedReport = "";

            using (var dst_file = new StreamReader(dstFileName)) 
            {
                writedReport = dst_file.ReadLine();
            }

            Assert.Equal(expectedReport.ToString(),writedReport);

            File.Delete(srcFileName);
            File.Delete(dstFileName);
        }
    }
}
