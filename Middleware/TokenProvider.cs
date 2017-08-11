using System;
using System.Threading.Tasks;
using MyTemplate.Persistence;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyTemplate.Models;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;

namespace MyTemplate.Middleware
{
    public class TokenProvider
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;
        //private readonly UserManager<ApplicationUser> _userManager;
         private readonly MyTemplateDbContext db;

        public TokenProvider(
        RequestDelegate next,
        IOptions<TokenProviderOptions> options,
        MyTemplateDbContext db)        
        {
            this._next = next;
            this.db = db;
            this._options = options.Value;
        }

        public Task Invoke(HttpContext context)
        {
            if(!context.Request.Path.Equals(_options.Path,StringComparison.Ordinal))
            {
                return NewMethod(context);
            }

            if (!context.Request.Method.Equals("POST") || !context.Request.HasFormContentType){
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad Request");
            }

            return GenerateToken(context);

        }

        private Task NewMethod(HttpContext context)
        {
            return _next(context);
        }

        private async Task GenerateToken(HttpContext context)
        {
            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];
            

            User user = Login(username,password);

            if(user == null){
                
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("{}");
                return;
            }

            var now  = DateTime.UtcNow;
            List<Claim> claims = new List<Claim>();
            
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, username));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64));
            

            List<Role> Roles = new List<Role>();
            Roles = db.Roles.Where(r=>r.UserId.Equals(user.Id)).ToList();

            
            foreach(Role r in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, r.RoleName));
            }

            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials
             );

             
          
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new 
            {
                token = encodedJwt,
                expires_ = (int)_options.Expiration.TotalSeconds
            };

            context.Response.ContentType ="application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings{Formatting = Formatting.Indented}));


        }

        public User Login(string username, string password)
        {
            byte[] salt = new byte[128 / 8];          

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
           password: password,
           salt: System.Text.Encoding.ASCII.GetBytes(username),
            prf: KeyDerivationPrf.HMACSHA1,
           iterationCount: 10000,
           numBytesRequested: 256 / 8));

            User User = new User();
            User = db.Users.Where(u => u.Username.Equals(username) && u.Password.Equals(hashed)).FirstOrDefault();           
         
            if (User != null)
            {
               return User;
            }
             return null;
        }


    }

    

    public class TokenProviderOptions
    {
        public string Path { get; set; } = "/user/login";
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(30);
        public SigningCredentials SigningCredentials { get; set; }
    }
}