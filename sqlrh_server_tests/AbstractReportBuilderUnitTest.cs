using System.IO;
using System;
using Xunit;
using Moq;
using System.Threading.Tasks;

namespace sqlrh_server_tests
{
    public class AbstractReportBuilderUnitTest
    {
        [Fact]
        public void WriteScalarTest()
        {
            var dbMock = new Mock<IExternalDataBaseRepository>();

            var sqlMock = new Mock<ISQLQueryExecutor>();

            TxtReportBuilder tb = new TxtReportBuilder(dbMock.Object, sqlMock.Object);

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

            var dbMock = new Mock<IExternalDataBaseRepository>();
            dbMock.Setup(a => a.GetConnectionString(It.IsNotNull<string>())).
                Returns(new Task<string>(() => "db1_cs"));

            object value = 1.23456789;

            string expected_result = " 1,2";

            var sqlMock = new Mock<ISQLQueryExecutor>();
            sqlMock.Setup(a => a.ExecuteScalar(It.IsNotNull<string>(),It.IsNotNull<string>())).
                Returns(value);

            TxtReportBuilder tb = new TxtReportBuilder(dbMock.Object, sqlMock.Object);

            string srcFileName = Path.Combine(Directory.GetCurrentDirectory(),"src3.txt");
            
            using (var src_file = new StreamWriter(srcFileName)) 
            {

            }

            using (var src = new StreamReader(srcFileName))
            {
                using (var dst = new StringWriter())
                {
                    tb.OpenFilesForMock(src,dst);

                    tb.ExecuteQuery(query);

                    string obtained_result = dst.ToString();
                    
                    Assert.Equal(expected_result,obtained_result);
                }
            }

            File.Delete(srcFileName);
        }
    }
}
