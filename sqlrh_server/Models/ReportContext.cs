using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
 
public class ReportContext : DbContext, IReportRepository
{
    private DbSet<Report> Reports { get; set; }

    public IEnumerable<Report> All => Reports;

    public ReportContext(DbContextOptions<ReportContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Report>().ToTable("Reports");
    }

    public Report Add(string name)
    {
        int c = Reports.Count();
        var n = Reports.Add(new Report() {Id = c + 1, Name = name}).Entity;
        SaveChanges();
        return n;
    }
}
