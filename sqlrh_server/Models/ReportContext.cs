using Microsoft.EntityFrameworkCore;
 
public class ReportContext : DbContext
{
    public DbSet<Report> Reports { get; set; }

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
