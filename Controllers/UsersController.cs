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
            var users = await db.Users.Include(u => u.Roles).ToListAsync();
            return mapper.Map<List<User>, List<UserResource>>(users);
        }

        [HttpPost("/api/test")]
        public string test()
        {   
            return "abc";
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
        [Authorize(Roles = "Administrator")]
        [HttpPost("/api/users/update")]
        public IActionResult UpdateUser([FromBody]UserResource UserResource)
        {
            var user = mapper.Map<UserResource, User>(UserResource);

            foreach(Role r in user.Roles){

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
            var user = await db.Users.Where(u => u.Id.Equals(id)).Include(u => u.Roles).SingleOrDefaultAsync();
            return mapper.Map<User, UserResource>(user);
            
        }

        [Authorize(ActiveAuthenticationSchemes="Bearer")]
        [Authorize(Roles = "Administrator")]
        [HttpGet("/api/users/delete")]
         public async Task<IEnumerable<UserResource>> DeleteUser(int id)
        {
            var user = await db.Users.Where(u => u.Id.Equals(id)).Include(u => u.Roles).SingleOrDefaultAsync();
            
            db.Users.Remove(user);
            var users = await db.Users.Include(u => u.Roles).ToListAsync();
            return mapper.Map<List<User>, List<UserResource>>(users);
        }
        

    }
}