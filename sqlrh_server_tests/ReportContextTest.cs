using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class ReportContextTest
{
    DbContextOptions<ReportContext> options;

    public ReportContextTest()
    {
        options = new DbContextOptionsBuilder<ReportContext>()
            .UseInMemoryDatabase(databaseName: "PeportTest")
            .Options;
    }

    [Fact]
    public void AddTest()
    {
        var rc = new ReportContext(options);
        for (int i = 0; i < 10; ++i)
        {
            string name = $"r{i}";
            var t = rc.Add(name);
            t.Wait();
            Assert.Equal(name,t.Result.Name);
        }
    }

    [Fact]
    public void GetAllTest()
    {

    }
}