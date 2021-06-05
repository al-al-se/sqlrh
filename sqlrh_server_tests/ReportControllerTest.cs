using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

public class ReportControllerTest
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
        IReportRepository reports = new ReportContext(reportContextOptions);
        IUserRepository users = new UserContext(userContextOptions);
        var builderServiceMock = new Mock<IReportBuilderService>();

        var rc = new sqlrh_server.Controllers.ReportsController(
                        LoggerMock.Object,reports, users, builderServiceMock.Object);

        string admin = "a";
        users.Add(new SqlrhUser(admin) {Admin = true});
        string user = "u";
        users.Add(new SqlrhUser(user) {Admin = false});

        string name = "r";
        var t = rc.AddNew(user,name);
        t.Wait();
        Assert.True(t.Result is UnauthorizedResult);
        
        t = rc.AddNew(admin,name);
        t.Wait();
        Assert.True(t.Result is CreatedResult);
        var created = t.Result as CreatedResult;
        Assert.True(created.Value is Report);
        var createdReport  = created.Value as Report;
        Assert.Equal(name,createdReport.Name);
    }
}