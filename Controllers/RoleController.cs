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

namespace MyTemplate.Controllers
{
    [Authorize(ActiveAuthenticationSchemes="Bearer")]
    public class RoleController
    {
        private readonly MyTemplateDbContext db;
        private readonly IMapper mapper;
        public RoleController(MyTemplateDbContext db, IMapper mapper)
        {
            this.mapper = mapper;
            this.db = db;
        }

        [HttpGet("/api/roles")]
        public async Task<IEnumerable<RoleListResource>> GetRoles()
        {
            List<RoleType> roles = await db.RoleTypes.ToListAsync();
            return mapper.Map<List<RoleType>, List<RoleListResource>>(roles);
        }
    }
}