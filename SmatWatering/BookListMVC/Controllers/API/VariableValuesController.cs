using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookListMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace SmartWatering.Controllers.API
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class VariableValuesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VariableValuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/variablevalue/all
        [HttpGet("all")]
        public ActionResult GetAllSensorValue()
        {
            var val1 = _context.VariableValue.Where(x => x.VariableId == 4).OrderByDescending(x => x.VariableValueId).Take(1).Select(x => new { Value = Math.Round(x.Value, 2) });
            var val2 = _context.VariableValue.Where(x => x.VariableId == 2).OrderByDescending(x => x.VariableValueId).Take(1).Select(x => new { Value = Math.Round(x.Value, 2) });
            var val3 = _context.VariableValue.Where(x => x.VariableId == 3).OrderByDescending(x => x.VariableValueId).Take(1).Select(x => new { Value = Math.Round(x.Value, 2) });
            var val4 = _context.VariableValue.Where(x => x.VariableId == 1).OrderByDescending(x => x.VariableValueId).Take(1).Select(x => new { Value = Math.Round(x.Value, 2) });
            var val = new { id1 = val1, id2 = val2, id3 = val3, id4 = val4 };
            //var val = new { id1 = val1 ,id2 = val2 };
            return new JsonResult(val);
        }
        //GET: api/SensorValues
        [HttpGet("{VariableId}")]
         public ActionResult GetSensorValue(int VariableId)
        {
            var val = _context.VariableValue.Where(x => x.VariableId == VariableId).OrderByDescending(x => x.VariableValueId).Take(1).Select(x => new { Value = Math.Round(x.Value , 2) });
            return new JsonResult(val);
        }
        //GET: api/variablevalue/Average
        [HttpGet("average")]
        public ActionResult Average()
        {
            int now = DateTime.Now.Day;
            var val = _context.VariableValue
                .Where(x => (x.VariableId == 1 || x.VariableId == 2 || x.VariableId == 3) && x.CreatedDate.Day == now)
                .GroupBy(x => x.VariableId)
                .Select(y => new { VariableId = y.Key, Quantity = Math.Round(y.Average(x => x.Value), 2) });
            return new JsonResult(val);
        }

        // GET: api/VariableValues
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<VariableValue>>> GetVariableValue()
        //{
        //    return await _context.VariableValue.ToListAsync();
        //}

        // GET: api/VariableValues/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<VariableValue>> GetVariableValue(int id)
        //{
        //    var variableValue = await _context.VariableValue.FindAsync(id);

        //    if (variableValue == null)
        //    {
        //        return View("NotFound");
        //    }

        //    return variableValue;
        //}

        // PUT: api/VariableValues/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVariableValue(int id, VariableValue variableValue)
        {
            if (id != variableValue.VariableValueId)
            {
                return BadRequest();
            }

            _context.Entry(variableValue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VariableValueExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/VariableValues
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<VariableValue>> PostVariableValue(List<VariableValue> variableValue)
        {
           foreach(var v in variableValue)
            {
                _context.VariableValue.Add(v);
                _context.SaveChanges();
            }
           
           return NoContent();
            //return CreatedAtAction("GetVariableValue", new { id = variableValue.VariableValueId }, variableValue);
        }

        // DELETE: api/VariableValues/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<VariableValue>> DeleteVariableValue(int id)
        {
            var variableValue = await _context.VariableValue.FindAsync(id);
            if (variableValue == null)
            {
                return NotFound();
            }

            _context.VariableValue.Remove(variableValue);
            await _context.SaveChangesAsync();

            return variableValue;
        }

        private bool VariableValueExists(int id)
        {
            return _context.VariableValue.Any(e => e.VariableValueId == id);
        }
    }
}
