using Projekt.Data.Models;
using System.Threading.Tasks;

namespace Projekt.Data.Repositories
{
    public interface IUserRepository
    {
        Task<DbResult<User>> AuthenticateUser(User u, string password);
        Task<DbResult<User>> CreateUser(User u, string password);
    }
}
