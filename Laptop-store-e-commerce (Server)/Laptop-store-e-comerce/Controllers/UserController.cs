using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Laptop_store_e_comerce.Models;
using Laptop_store_e_comerce.Services;
using Laptop_store_e_comerce.Repository;

namespace Laptop_store_e_comerce.Controllers
{
    [Route("data/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly StoreContext database;
        UserRepository _userRepository;
        private UserService userSer;
        public UserController(StoreContext context)
        {
            database = context;
            _userRepository = new UserRepository(context);
            userSer = new UserService(_userRepository);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserByID1(string id)
        {
            var user = await database.Users.Include(user => user.CartDetails)
                                .ThenInclude(cart => cart.IdProductNavigation)
                                .ThenInclude(pro => pro.IdloaiNavigation)
                        .Include(user => user.Bills).FirstOrDefaultAsync(user => user.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }
        [HttpGet("test/{id}")]
        public async Task<ActionResult<User>> GetUserByIDTest(string id)
        {
            if (id != null)
            {
               return await userSer.getUserById(id);
            }
            else return BadRequest();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            //return await database.Users.ToListAsync();
            if (userSer.getUsers().Count == 0) return NotFound();
            else return userSer.getUsers();
        }
        [HttpGet("id={id}")]
        public async Task<ActionResult<List<User>>> GetUserByID2(string id)
        {
            List<User> users = await database.Users.Include(user => user.CartDetails).Include(user => user.Bills).Where(user => user.Id == id).ToListAsync();
            if (users.Count == 0)
            {
                return NotFound();
            }
            return users;
        }
        [HttpGet("name={value}")]
        public async Task<ActionResult<List<User>>> getUserByName(string value)
        {
            try
            {
                List<User> users =new List<User>();
                await database.Users.ForEachAsync(x =>
                {
                    if (checkName(x, value)) users.Add(x);
                });
                if (users.Count != 0) return users;
                else return NotFound();
            } catch (Exception e)
            { Console.WriteLine(e.ToString()); return BadRequest(); }
        }
        [HttpGet("customer/name={value}")]
        public async Task<ActionResult<List<User>>> getCustomerByName(string value)
        {
            try
            {
                List<User> users = new List<User>();
                await database.Users.ForEachAsync(x =>
                {
                    if (checkName(x, value) && x.Mode == "CUSTOMER") users.Add(x);
                });
                if (users.Count != 0) return users;
                else return NotFound();
            }
            catch (Exception e)
            { Console.WriteLine(e.ToString()); return BadRequest(); }
        }
        [HttpGet("login/{email}/{pass}")]
        public async Task<ActionResult<User>> Login(string email, string pass)
        {
            var user = await database.Users.Include(user => user.Bills).ThenInclude(bill => bill.BillDetails)
                                           .Include(user => user.CartDetails).ThenInclude(cart => cart.IdProductNavigation)
                                                                             .ThenInclude(cart => cart.IdloaiNavigation)
                                           .FirstOrDefaultAsync(a => a.Email == email);
            if (user != null)
            {
                if (pass != user.Pass) return BadRequest();
                else return user;
            }
            else return NotFound();
        }
        [HttpGet("email={value}")]
        public async Task<ActionResult<List<User>>> getUserByEmail(string value)
        {
            try {
                List<User> users = await database.Users.Where(user => user.Email == value).ToListAsync();
                if (users.Count != 0) return users;
                else return NotFound();
            } catch (Exception e) { Console.WriteLine(e.ToString()); return BadRequest(); }
        }
        [HttpGet("sdt={value}")]
        public async Task<ActionResult<List<User>>> getUserBySdt(string value)
        {
            try
            {
                List<User> users = await database.Users.Where(user => user.Sdt == value).ToListAsync();
                if (users.Count != 0) return users;
                else return NotFound();
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); return BadRequest(); }
        }
        [HttpGet("customer/email={value}")]
        public async Task<ActionResult<List<User>>> getCustomerByEmail(string value)
        {
            try
            {
                List<User> users = await database.Users.Where(user =>user.Mode == "CUSTOMER" && user.Email == value).ToListAsync();
                if (users.Count != 0) return users;
                else return NotFound();
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); return BadRequest(); }
        }
        [HttpGet("customer/id={value}")]
        public async Task<ActionResult<List<User>>> getCustomerByID(string value)
        {
            try
            {
                List<User> users = await database.Users.Where(user => user.Mode == "CUSTOMER" && user.Id == value).ToListAsync();
                if (users.Count != 0) return users;
                else return NotFound();
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); return BadRequest(); }
        }

        [HttpGet("mode={value}")]
        public async Task<ActionResult<List<User>>> getUserByMode(string value)
        {
            List<User> users = await database.Users.Where(user => user.Mode == value).ToListAsync();
            if (users.Count == 0) return NotFound();
            else return users;
        }
        [HttpPut]
        public async Task<IActionResult> PutUser(User user)
        {
            database.Entry(user).State = EntityState.Modified;
            try
            {
                await database.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!UserExists(user.Id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }
            return CreatedAtAction("GetUserByID", new { id = user.Id }, user);
        }
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (existEmail(user.Email)) return Conflict();
            database.Users.Add(user);
            try
            {
                await database.SaveChangesAsync();
            }
            catch (DbUpdateException e) { Console.WriteLine(e.ToString()); return BadRequest(); }
            return CreatedAtAction("GetUserByID", new { id = user.Id }, user);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await database.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            database.Users.Remove(user);
            await database.SaveChangesAsync();
            return user;
        }
        private bool UserExists(string id)
        {
            return database.Users.Any(e => e.Id == id);
        }
        private bool existEmail(string email)
        {
            return database.Users.Any(user => user.Email == email);
        }
        private bool checkName(User user ,String value)
        {
            String fullName = user.Firstname + " " + user.Lastname;
            String fullName2 = user.Lastname + " " + user.Firstname;
            if ((fullName.Contains(value) || fullName == value) || (fullName2.Contains(value) || fullName2 == value)) return true;
            return false;
        }
        private object loginValidate(string email,string pass)
        {
            bool result = true;
            if(email == null || pass == null)  return "Email or password is empty";
            
            if (!email.Contains('@')) return "Email is must contain @ character";
            else
            {
                if (email.Contains(' ')) return "Email is not allow to include";
            }
            if (!checkUppercase(pass)) return "Password must contain least 1 Uppercase character";
            if (pass.Length < 8) return "Password must have least 8 character of length";
            if (!checkCharacterIsNumber(pass)) return "Password must contain least 1 character is Number";
            return result;
        }
        private bool checkUppercase(string pass)
        {
            bool result = false;
            for(int i = 0; i < pass.Length; i++)
            {
                if (Char.IsUpper(pass[i])) result = true;
                break;
            }
            return result;
        }
        private bool checkCharacterIsNumber(string pass)
        {
            bool result = false;
            for(int i = 0;i < pass.Length; i++)
            {
                if (Char.IsDigit(pass[i])) result = true;
                break;
            }
            return result;
        }
    }
}
