using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 
public class ReportContext : DbContext, IReportRepository
{
    private DbSet<Report> Reports { get; set; }

    private DbSet<SqlrhUser> Users {get; set;}

    private DbSet<AccessRule> Access {get; set;}

    private DbSet<Shedule> Shedules {get; set;}


    public ReportContext(DbContextOptions<ReportContext> options)
        : base(options)
    {
    }

    public async Task<bool> IsReportAvailableToUser(int id, string login)
    {
        if (await UserContext.IsUserAdmin(Users,login)) return true;

        if (await Access.AnyAsync(
                    a => (a.ReportTemplate.Id == id && a.AdmittedUser.Login == login)))
            return true;
        
        return false;
    }

    public  async Task<IEnumerable<Report>> GetAll(string login)
    {
        if (await UserContext.IsUserAdmin(Users,login)) 
                 return await Reports.ToListAsync();
        
        return Access.Where(a => a.AdmittedUser.Login == login)
                        .Select(a => a.ReportTemplate);
    }

    public delegate Task<bool> ContainsDelegate(int id);

    protected Random RandomGenerator = new Random();
    public async  Task<int> GenerateNewId(ContainsDelegate contains)
    {
        // sqlite has not sequences
        for (int i = 0; i < 5000; i++)
        {
            int v = RandomGenerator.Next();
            if (!await contains(v))
            {
                return v;
            }
        }
        throw new ArgumentOutOfRangeException();
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
                                    Id = await GenerateNewId(ContainsId),
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

    public async Task<bool> CantainsAccessRuleId(int id)
    {
        return await Access.AnyAsync(a => a.Id == id);
    }

    public async Task<AccessRule> Allow(int id, string login)
    {
        var report = await GetReport(id);
        var user = await UserContext.Get(Users,login);
        var n = new AccessRule() 
        {
            Id = await GenerateNewId(CantainsAccessRuleId),
            ReportTemplate = report,
            AdmittedUser = user
        };
        await Access.AddAsync(n);
        await SaveChangesAsync();
        return n;
    }

    public async Task<AccessRule> Disallow(int id, string login)
    {
        var ar = await Access.FirstAsync(
            a => a.ReportTemplate.Id == id && a.AdmittedUser.Login == login);
        Access.Remove(ar);
        await SaveChangesAsync();
        return ar;
    }
}
