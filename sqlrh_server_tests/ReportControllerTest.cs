using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

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
    void AddNewTest()
    {
        var reports = new ReportContext(reportContextOptions);
        var users = new UserContext(userContextOptions);
        var builderServiceMock = new Mock<IReportBuilderService>();

        var rc = new sqlrh_server.Controllers.ReportsController(
                        LoggerMock.Object,reports, users, builderServiceMock.Object);

        string name = "r";
        //authorize?
        var t = rc.AddNew(name);
        t.Wait();
        Assert.True(t.Result is CreatedResult);
        var created = t.Result as CreatedResult;
        Assert.True(created.Value is Report);
        var createdReport  = created.Value as Report;
        Assert.Equal(name,createdReport.Name);
    }
}