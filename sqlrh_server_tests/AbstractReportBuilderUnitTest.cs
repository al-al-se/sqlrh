using System;
using Xunit;
using Moq;

namespace sqlrh_server_tests
{
    public class AbstractReportBuilderUnitTest
    {
        [Fact]
        public void WriteScalarTest()
        {
            var mock = new Mock<IExternalDataBaseRepository>();

            TxtReportBuilder tb = new TxtReportBuilder(mock.Object);
        }
    }
}
