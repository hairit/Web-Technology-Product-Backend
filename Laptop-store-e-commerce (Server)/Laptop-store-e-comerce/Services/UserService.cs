using Laptop_store_e_comerce.Models;
using Laptop_store_e_comerce.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laptop_store_e_comerce.Services
{
    public class UserService
    {
        private IUserRepository userRepo;

        public UserService(IUserRepository userRepository)
        {
            this.userRepo = userRepository;
        }

        public async Task<ActionResult<User>> getUserById(string id)
        {
            var user = await userRepo.getUserByID(id);
            return user;
        }
        public List<User> getUsers()
        {
            List<User> users =  userRepo.getUsers();
            return users;
        }
    }
}
