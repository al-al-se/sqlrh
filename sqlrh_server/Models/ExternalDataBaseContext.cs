using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 
public class ExternalDataBaseContext : DbContext, IExternalDataBaseRepository
{
    private DbSet<ExternalDatabase> Bases { get; set; }

        public ExternalDataBaseContext(DbContextOptions<ExternalDataBaseContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExternalDatabase>().ToTable("ExternalDatabases");
    }

    public  async Task<IEnumerable<ExternalDatabase>> GetAll()
    {
        var r = await Bases.ToListAsync();
        return r;
    }

    public async Task<string> GetConnectionString(string alias)
    {
        return (await Bases.FirstAsync(b => b.Alias == alias)).connectionString;
    }
}