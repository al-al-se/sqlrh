using System;
using Xunit;
using Moq;

namespace sqlrh_server_tests
{
    public class TxtReportBuilderUnitTest
    {
        [Fact]
        public void Test1()
        {
            var mock = new Mock<IExternalDataBaseRepository>();

            TxtReportBuilder tb = new TxtReportBuilder(mock.Object);
        }
    }
}
