using Laptop_store_e_comerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laptop_store_e_comerce.Repository
{
    public interface IUserRepository
    {
         Task<ActionResult<User>> getUserByID(string id);
         List<User> getUsers();
    }

    public class UserRepository : IUserRepository
    {
        public readonly StoreContext context;
        public UserRepository(StoreContext database)
        {
            this.context = database;
        }
        public virtual async Task<ActionResult<User>> getUserByID(string id)
        {
            User user = await context.Users.FirstOrDefaultAsync(user => user.Id == id);
            return user;
        }
        public virtual async Task<ActionResult<List<User>>> getUsersAsync()
        {
            return await context.Users.FromSqlRaw("select * from usersView").ToListAsync();
        }

        public virtual List<User> getUsers()
        {
            List<User> users = new List<User>();
            users = context.Users.FromSqlRaw("select * from usersView").ToList();
            return users;
        }
    }
}
