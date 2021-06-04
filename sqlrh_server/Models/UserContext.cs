using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 
public class UserContext : DbContext, IUserRepository
{
    private DbSet<SqlrhUser> Users { get; set; }

        public UserContext(DbContextOptions<UserContext> options)
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

    public  async Task<int> Count()
    {
        return await Users.CountAsync();
    }
    public  async Task<IEnumerable<SqlrhUser>> GetAll()
    {
        var r = await Users.ToListAsync();
        return r;
    }

    public async Task<SqlrhUser> Get(string login)
    {
        try{
            return await Users.FirstAsync(i => i.Login == login);
        } catch (InvalidOperationException)
        {
            throw new InvalidOperationException($"User with login {login} not found");
        }
    }

    public async Task<bool> Contains(string login)
    {
        return await Users.AnyAsync(i => i.Login == login);
    }

    public async Task<SqlrhUser> Add(SqlrhUser nu)
    {
        var n =  (await Users.AddAsync(nu)).Entity;
        await SaveChangesAsync();
        return n;
    }

    public async Task<SqlrhUser> Update(SqlrhUser u)
    {
        var e = await Get(u.Login);
        e.Copy(u);
        await SaveChangesAsync();
        return e;
    }

    public async Task Delete(string login)
    {
        var u = await Get(login);
        Users.Remove(u);
        await SaveChangesAsync();
    }
}