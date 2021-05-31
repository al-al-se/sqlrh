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
            
            using (var src_file = new StreamWriter(srcFileName)) 
            {

            }

            string obtained_result = "";

            using (var src = new StreamReader(srcFileName))
            {
                using (var dst = new StringWriter())
                {

                    tb.OpenFilesForMock(src,dst);

                    int val = 12;
                    tb.WriteScalar(val,":d");

                    obtained_result = dst.ToString();
                }
            }

            File.Delete(srcFileName);

            string expected_result = "12";

            Assert.Equal(obtained_result,expected_result);
        }
    }
}
