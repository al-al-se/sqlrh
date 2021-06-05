using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

class ReportControllerTest
{
    DbContextOptions<ReportContext> reportContextOptions;

    DbContextOptions<UserContext> userContextOptions;
     Mock<ILogger<sqlrh_server.Controllers.ReportsController>> LoggerMock =
      new Mock<ILogger<sqlrh_server.Controllers.ReportsController>>();

    public ReportControllerTest()
    {
        reportContextOptions = new DbContextOptionsBuilder<ReportContext>()
            .UseInMemoryDatabase(databaseName: "Test")
            .Options;

        userContextOptions = new DbContextOptionsBuilder<UserContext>()
            .UseInMemoryDatabase(databaseName: "Test")
            .Options;
    }
    

    [Fact]
    void Test()
    {
        var reports = new ReportContext(reportContextOptions);
        var users = new UserContext(userContextOptions);
        var builderServiceMock = new Mock<IReportBuilderService>();

        var rc = new sqlrh_server.Controllers.ReportsController(
                        LoggerMock.Object,reports, users, builderServiceMock.Object);
    }
}