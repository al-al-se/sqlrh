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
       
    }
    public  async Task<IEnumerable<SqlrhUser>> GetAll()
    {
        var r = await Users.ToListAsync();
        return r;
    }

    public async Task<SqlrhUser> Get(int id)
    {
        try{
            return await Users.FirstAsync(i => i.Id == id);
        } catch (InvalidOperationException)
        {
            throw new InvalidOperationException($"User with Id = {id} not found");
        }
    }

    public async Task<bool> Contains(int id)
    {
        return await Users.AnyAsync(i => i.Id == id);
    }

    public async Task<SqlrhUser> Add(SqlrhUser nu)
    {
        var n =  (await Users.AddAsync(nu)).Entity;
        await SaveChangesAsync();
        return n;
    }

    public async Task<SqlrhUser> Update(SqlrhUser u)
    {
        var e = await Get(u.Id);
        e.Copy(u);
        await SaveChangesAsync();
        return e;
    }

    public async Task Delete(int id)
    {
        var u = await Get(id);
        Users.Remove(u);
        await SaveChangesAsync();
    }
}