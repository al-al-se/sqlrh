using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 
public class ExternalDataBaseContext : DbContext, IExternalDataBaseRepository
{
    private DbSet<ExternalDatabase> ExternalDatabases { get; set; }

        public ExternalDataBaseContext(DbContextOptions<ExternalDataBaseContext> options)
        : base(options)
    {
       
    }
    public  async Task<IEnumerable<ExternalDatabase>> GetAll()
    {
        var r = await ExternalDatabases.ToListAsync();
        return r;
    }

    public async Task<ExternalDatabase> Get(string alias)
    {
        try{
            return await ExternalDatabases.FirstAsync(i => i.Alias == alias);
        } catch (InvalidOperationException)
        {
            throw new InvalidOperationException($"Database with alias = {alias} not found");
        }
    }

    public async Task<string> GetConnectionString(string alias)
    {
        return (await Get(alias)).ConnectionString;
    }

    public async Task<ExternalDatabase> Add(string alias,
                                            string dbms,
                                            string connectionString)
    {
        var n =  (await ExternalDatabases.AddAsync(
            new ExternalDatabase() {
                                    Alias = alias,
                                    DBMS = dbms,
                                    ConnectionString = connectionString}
                                )).Entity;
        await SaveChangesAsync();
        return n;
    }

    public async Task<bool> Contains(string alias)
    {
        return await ExternalDatabases.AnyAsync(i => i.Alias == alias);
    }

    public async Task<ExternalDatabase> Change(string alias, string connectionString)
    {
        var b = await Get(alias);
        b.ConnectionString = connectionString;
        await SaveChangesAsync();
        return b;
    }
}