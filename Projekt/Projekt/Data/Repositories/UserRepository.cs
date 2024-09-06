using Microsoft.EntityFrameworkCore;
using Projekt.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Projekt.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;

        public UserRepository(DataContext context)
        {
            this.context = context;
        }
        private async Task<User> GetUser(string username) => await context.Users.FirstOrDefaultAsync(x => x.UserName == username);

        public async Task<DbResult<User>> AuthenticateUser(User user, string password)
        {
            user = await GetUser(user.UserName);
            if(user == null)
            {
                return DbResult<User>.CreateFail("User does not exist");
            }

            HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);
            byte[] computedHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));
            if(!CompareHash(computedHash, user.PasswordHash))
            {
                return DbResult<User>.CreateFail("Password is not correct");
            }

            return DbResult<User>.CreateSuccess("Authentication success", user);
        }

        public async Task<DbResult<User>> CreateUser(User u, string password)
        {
            if (await GetUser(u.UserName) != null)
            {
                return DbResult<User>.CreateFail("Username already exist!");
            }

            HMACSHA512 hmac = new HMACSHA512();
            u.PasswordSalt = hmac.Key;
            u.PasswordHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));
            await context.AddAsync(u);
            await context.SaveChangesAsync();
            return DbResult<User>.CreateSuccess("User created!", u);
        }

        private static bool CompareHash(byte[] h1, byte[] h2)
        {
            return h1.Select((x, i) => h1[i] == h2[i]).All(x => x);
        }
    }
}
