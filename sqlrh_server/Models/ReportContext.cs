using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 
public class ReportContext : DbContext, IReportRepository
{
    private DbSet<Report> Reports { get; set; }

    public  async Task<IEnumerable<Report>> GetAll()
    {
        var r = await Reports.ToListAsync();
        return r;
    }

    public ReportContext(DbContextOptions<ReportContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Report>().ToTable("Reports");
        modelBuilder.Entity<ExternalDatabase>().ToTable("ExternalDatabases");
    }

    public async Task<Report> Add(string name)
    {
        int c = await Reports.CountAsync();
        // sqlite has not sequences
        // collision unlikely
        var n = ( await Reports.AddAsync(new Report() {Id = c + 1, Name = name})).Entity;
        await SaveChangesAsync();
        return n;
    }

    public async Task<bool> ContainsId(int id)
    {
        return await Reports.AnyAsync(r => r.Id == id);
    }

    public async Task<Report> GetReport(int id)
    {
        return await Reports.FirstAsync(r => r.Id == id);
    }

    public async Task<Report> LoadFile(int id, string path)
    {
       var r =  await GetReport(id);
       r.FilePath = path;
       await SaveChangesAsync();
       return r;
    }
}
