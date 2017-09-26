using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTemplate.Controllers.Resources;
using MyTemplate.Models;
using MyTemplate.Persistence;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;
using BackEnd.Controllers.Resources;

namespace MyTemplate.Controllers
{
    
    public class UsersController : Controller
    {
        private readonly MyTemplateDbContext db;
        private readonly IMapper mapper;
        public UsersController(MyTemplateDbContext db, IMapper mapper)
        {
            this.mapper = mapper;
            this.db = db;

        }

            
        [Authorize(ActiveAuthenticationSchemes="Bearer")]
        [Authorize(Roles = "Administrator")]
        [HttpGet("/api/users")]
        public async Task<IEnumerable<UserResource>> GetUsers()
        {
            var users = await db.Users.Include(u => u.Roles).Include(u => u.Scopes).ToListAsync();
            return mapper.Map<List<User>, List<UserResource>>(users);
        }
        
        [Authorize(ActiveAuthenticationSchemes="Bearer")]
        [Authorize(Roles = "Administrator")]
        [HttpPost("/api/users/create")]
        public IActionResult CreateUser([FromBody]UserResource UserResource)
        {
            var user = mapper.Map<UserResource, User>(UserResource);
            
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create()){
                 rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: user.Password,
            salt: System.Text.Encoding.ASCII.GetBytes(user.Username),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
            
            user.Password = hashed;
            db.Users.Add(user);
            db.SaveChanges();
            return Ok(mapper.Map<User, UserResource>(user));
        }

        [Authorize(ActiveAuthenticationSchemes="Bearer")]
        [HttpPost("/api/users/changepassword")]
        public IActionResult ChangePass([FromBody]ChangePassResource changepass) {

            User User = new User();
            User = db.Users.Where(u => u.Username.Equals(changepass.username) && u.Password.Equals(hashed(changepass.username, changepass.oldpassword))).FirstOrDefault();           
         
            if (User != null)
            {
               User.Password = hashed(User.Username, changepass.newpassword);
               db.SaveChanges();
            }
              return Ok(User);
        }

        public string hashed(string username, string password) {
            
            byte[] salt = new byte[128 / 8]; 
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: System.Text.Encoding.ASCII.GetBytes(username),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

            return hashed;
        }


        [Authorize(ActiveAuthenticationSchemes="Bearer")]
        [Authorize(Roles = "Administrator")]
        [HttpPost("/api/users/update")]
        public IActionResult UpdateUser([FromBody]UserResource UserResource)
        {
            List<string> roles = new List<string>();
            roles.Add("Administrator");
            roles.Add("Staff");

            var user = mapper.Map<UserResource, User>(UserResource);
            var userdb = db.Users.Where(u => u.Id.Equals(user.Id)).Include(u => u.Roles).Include(u => u.Scopes).SingleOrDefault();
            userdb.Email = user.Email;
            db.SaveChanges();

            foreach(string r in roles){
                if(user.Roles.Where( rl => rl.RoleName.Equals(r)).Count() < 1 ) {
                   Role roledb = db.Roles.Where(rl => rl.RoleName.Equals(r) && rl.User.Id.Equals(user.Id)).SingleOrDefault();
                    if (roledb != null) {     
                        db.Roles.Remove(roledb);
                        db.SaveChanges();
                    }
                }else{
                    if(userdb.Roles.Where(r1=>r1.RoleName.Equals(r) && r1.User.Id.Equals(user.Id)).Count() < 1){
                        Role roledb = new Role();
                        roledb.UserId = user.Id;
                        roledb.RoleName = r;
                        db.Roles.Add(roledb);
                        db.SaveChanges();
                    }
                }
            }
            
            List<Location> locations = db.Locations.ToList();
     
            foreach (Location l in locations) {
                if (user.Scopes.Where(s => s.Name.Equals(l.Name)).Count() < 1) {
                    Scope scopedb = db.Scopes.Where(s => s.Name.Equals(l.Name) && s.User.Id.Equals(user.Id)).SingleOrDefault();
                    if (scopedb != null) {
                        db.Scopes.Remove(scopedb);
                        db.SaveChanges();
                    }
                } else {
                    if (userdb.Scopes.Where(s => s.Name.Equals(l.Name) && s.User.Id.Equals(user.Id)).Count() < 1) {
                       Scope scopedb = new Scope();
                       scopedb.UserId = user.Id;
                       scopedb.Name = l.Name;
                       db.Scopes.Add(scopedb);
                       db.SaveChanges();     
                    }
                }
            }

            return Ok(mapper.Map<User, UserResource>(user));
        }

        [HttpGet("/api/users/usernamevalidation")]
        public IActionResult UsernameValidation([FromQuery]string username){
            var users = db.Users.Where(u=>u.Username.Equals(username)).ToList();
            if(users.Count> 0) return Ok(users);

            return Ok(null);
        }

        [Authorize(ActiveAuthenticationSchemes="Bearer")]
        [Authorize(Roles = "Administrator")]
        [HttpGet("/api/users/edit")]
        public async Task<UserResource> GetUser(int id)
        {
            var user = await db.Users.Where(u => u.Id.Equals(id)).Include(u => u.Roles).Include(u => u.Scopes).SingleOrDefaultAsync();
            return mapper.Map<User, UserResource>(user);
            
        }

        [Authorize(ActiveAuthenticationSchemes="Bearer")]
        [Authorize(Roles = "Administrator")]
        [HttpDelete("/api/users/delete")]
         public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await db.Users.Where(u => u.Id.Equals(id)).Include(u => u.Roles).SingleOrDefaultAsync();
            
            db.Users.Remove(user);
            try{
                await db.SaveChangesAsync();
                return Ok(id);
            }catch{
                return StatusCode(400);
            }
            
        }


    }
}