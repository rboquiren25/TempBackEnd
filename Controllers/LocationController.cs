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
    [Authorize(Roles = "Administrator")]
    public class LocationController : Controller
    {
        private readonly MyTemplateDbContext db;
        private readonly IMapper mapper;
        public LocationController(MyTemplateDbContext db, IMapper mapper)
        {
            this.mapper = mapper;
            this.db = db;
        }

      
        [HttpGet("/api/locations")]
        public async Task<IEnumerable<LocationResource>> GetLocations()
        {
            List<Location> locations = await db.Locations.ToListAsync();
            return mapper.Map<List<Location>, List<LocationResource>>(locations);
        }

        
        [HttpPost("/api/locations/create")]
        public IActionResult CreateLocation([FromBody]LocationResource LocationResource)
        {
            var location = mapper.Map<LocationResource, Location>(LocationResource);
            try {
                db.Locations.Add(location);
                db.SaveChanges();
            return Ok(mapper.Map<Location, LocationResource>(location));
            } catch {
                 return StatusCode(501);
            }
        }

        [HttpDelete("/api/locations/delete")]
         public async Task<IActionResult> DeleteLocation(int id)
        {
            var location = await db.Locations.Where(l => l.Id.Equals(id)).SingleOrDefaultAsync();
            
            db.Locations.Remove(location);
            try{
                await db.SaveChangesAsync();
                return Ok(id);
            }catch{
                return StatusCode(400);
            }
        }

        [HttpGet("/api/locations/edit")]
        public async Task<LocationResource> GetLocation(int id)
        {
            var location = await db.Locations.Where(l => l.Id.Equals(id)).SingleOrDefaultAsync();
            return mapper.Map<Location, LocationResource>(location);
        }

        
        [HttpPost("/api/locations/update")]
        public IActionResult UpdateLocation([FromBody]LocationResource LocationResource)
        {
            var location = mapper.Map<LocationResource, Location>(LocationResource);
            var locationdb = db.Locations.Where(l => l.Id.Equals(location.Id)).SingleOrDefault();
            locationdb.Name = location.Name;   
            db.SaveChanges();
            return Ok(mapper.Map<Location, LocationResource>(location));
        }

    }

}