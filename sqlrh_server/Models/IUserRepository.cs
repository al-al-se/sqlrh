using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserRepository
{
    Task<IEnumerable<SqlrhUser>> GetAll();

    Task<SqlrhUser> Add(SqlrhUser newUser);

    Task<bool> Contains(int id);

    Task<SqlrhUser> Update(SqlrhUser u);

    Task Delete(int id);
}