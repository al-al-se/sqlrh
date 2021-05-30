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

            var srcMock = new Mock<TextReader>();
            var dstMock = new Mock<TextWriter>();

            tb.OpenFilesForMock(srcMock,dstMock);

            int val = 12;
            tb.WriteScalar(val,":d");
        }
    }
}
