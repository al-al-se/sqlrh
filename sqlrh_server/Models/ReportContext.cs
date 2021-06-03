using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 
public class ReportContext : DbContext, IReportRepository
{
    private DbSet<Report> Reports { get; set; }

    private DbSet<AccessRule> Access {get; set;}

    private DbSet<Shedule> Shedules {get; set;}

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
        modelBuilder.Entity<AccessRule>().ToTable("Access");
        modelBuilder.Entity<SqlrhUser>().ToTable("Users");
        modelBuilder.Entity<Shedule>().ToTable("Shedule");
    }

    public async  Task<int> GenerateNewId()
    {
        // sqlite has not sequences
        int max = await Reports.MaxAsync(r => r.Id);
        int count = await Reports.CountAsync();

        if (2 * count < max)
        {
            for (int i = 0; i < max; i++)
            {
                if (!await Reports.AnyAsync(r => r.Id == i))
                {
                    return i;
                }
            }
        }

        return max + 1;
    }


    public async Task<bool> ContainsId(int id)
    {
        return await Reports.AnyAsync(r => r.Id == id);
    }

    public async Task<Report> Add(string name)
    {
        var n = ( await Reports.AddAsync(
                                new Report() 
                                {
                                    Id = await GenerateNewId(),
                                    Name = name
                                })).Entity;
        await SaveChangesAsync();
        return n;
    }

    public async Task<Report> GetReport(int id)
    {
        try{
            return await Reports.FirstAsync(r => r.Id == id);
        } catch (InvalidOperationException)
        {
            throw new InvalidOperationException($"Report template with id = {id} not found");
        }
    }

    public async Task<Report> LoadFile(int id, string path)
    {
       var r =  await GetReport(id);
       r.FilePath = path;
       await SaveChangesAsync();
       return r;
    }

    public async Task Delete(int id)
    {
        var b = await GetReport(id);
        Reports.Remove(b);
        await SaveChangesAsync();
    }
}
