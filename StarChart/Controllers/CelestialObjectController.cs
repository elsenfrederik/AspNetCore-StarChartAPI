using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
            {
                return NotFound();
            }
            celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name).ToList();
            if (celestialObjects.Count==0)
            {
                return NotFound();
            }

            foreach (var celobject in celestialObjects)
            {
                celobject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celobject.Id).ToList();
            }

            //celestialObjects.ToList().ForEach(c => c.Satellites = _context.CelestialObjects.Where(s => s.OrbitedObjectId == s.Id).ToList());


            return Ok(celestialObjects);
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            if (celestialObjects.Any())
            {
                return NotFound();
            }

            foreach (var celobject in celestialObjects)
            {
                celobject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celobject.Id).ToList();
            }

            return Ok(celestialObjects);
        }
        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new {id = celestialObject.Id}, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {

            var celestialObjectUpdateFind = _context.CelestialObjects.Find(id);
            if (celestialObjectUpdateFind == null)
            {
                return NotFound();
            }

            celestialObjectUpdateFind.Name = celestialObject.Name;
            celestialObjectUpdateFind.OrbitedObjectId = celestialObject.OrbitedObjectId;
            celestialObjectUpdateFind.OrbitalPeriod = celestialObject.OrbitalPeriod;
            _context.CelestialObjects.Update(celestialObjectUpdateFind);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {

            var celestialObjectUpdateFind = _context.CelestialObjects.Find(id);
            if (celestialObjectUpdateFind == null)
            {
                return NotFound();
            }

            celestialObjectUpdateFind.Name = name;
            _context.CelestialObjects.Update(celestialObjectUpdateFind);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

            var celestialObjects = _context.CelestialObjects.Where(c=> c.OrbitedObjectId == id || c.Id == id) ;
            if (celestialObjects.Any())
            {
                return NotFound();
            }
            
            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
