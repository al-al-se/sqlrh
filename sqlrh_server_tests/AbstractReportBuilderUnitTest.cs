using System.IO;
using System;
using Xunit;
using Moq;

namespace sqlrh_server_tests
{
    public class AbstractReportBuilderUnitTest
    {
        [Fact]
        public void WriteScalarIntTest()
        {
            var dbMock = new Mock<IExternalDataBaseRepository>();

            TxtReportBuilder tb = new TxtReportBuilder(dbMock.Object);

            string srcFileName = Path.Combine(Directory.GetCurrentDirectory(),"src.txt");
            File.Create(srcFileName);

            var src = new StreamReader(srcFileName);
            var dst = new StringWriter();

            tb.OpenFilesForMock(src,dst);

            int val = 12;
            tb.WriteScalar(val,":d");

            string obtained_result = dst.ToString();

            src.Close();
            File.Delete(srcFileName);

            string expected_result = "12";

            Assert.Equal(obtained_result,expected_result);
        }
    }
}
