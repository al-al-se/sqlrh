using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
 
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
}
