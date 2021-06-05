using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserRepository
{
    Task<IEnumerable<SqlrhUser>> GetAll();

    Task<int> Count();

    Task<SqlrhUser> Add(SqlrhUser newUser);

    Task<bool> IsUserAdmin(string userLogin);

    Task<bool> Contains(string login);

    Task<SqlrhUser> Get(string login);

    Task<SqlrhUser> Update(SqlrhUser u);

    Task Delete(string login);
}