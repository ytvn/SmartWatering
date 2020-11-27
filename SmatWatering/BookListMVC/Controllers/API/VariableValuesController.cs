using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookListMVC.Models;
using Microsoft.AspNetCore.Authorization;
using SmartWatering.Models.ViewModels;
using Nancy.Json;
using Newtonsoft.Json;
using SmartWatering.Util;

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
            var val = _context.VariableValue.Where(x => x.VariableId == VariableId).OrderByDescending(x => x.VariableValueId).Take(1).Select(x => new { Value = Math.Round(x.Value, 2) });
            return new JsonResult(val);
        }
        //GET: api/variablevalues/Average/5 // id of device
        [HttpGet("average/{id}")]
        public ActionResult Average(int type, int id)
        {
            var variables = from v in _context.Variable
                                 join dp in _context.DevicePin on v.PinId equals dp.PinId
                                 join vv in _context.VariableValue on v.VariableId equals vv.VariableId
                                 where dp.PinType == PinType.IN && dp.chipId == id
                                 select vv;
            int now = DateTime.Now.Day;
            var val = _context.VariableValue
                        .Where(x =>  x.CreatedDate.Day == now)
                        .GroupBy(x => x.VariableId)
                        .Select(y => new { VariableId = y.Key, Average = Math.Round(y.Max(x => x.Value), 2) });
            return new JsonResult(val);
        }

        //GET: api/VariableValues/Get/{Token}
        [HttpGet("Get/{Token}")]
        public async Task<IActionResult> GetVariableValue(string Token)
        {
            //var variableValue = await _context.VariableValue.FindAsync(id);
            var variableValues = _context.VariableValue;
            var variables = _context.Variable;
            var devicePin = _context.DevicePin;

            var result = await (from vv in variableValues
                                join v in variables on vv.VariableId equals v.VariableId
                                join dp in devicePin on v.PinId equals dp.PinId
                                join d in _context.Device on dp.chipId equals d.ChipId
                                where d.ReadAPIKey == Token
                                select new VariableValueView
                                {
                                    VariableValueId = vv.VariableValueId,
                                    Value = vv.Value,
                                    VariableName = v.VariableName,
                                    PIN = dp.PIN,
                                    ChipId = dp.chipId,
                                    CreatedBy = v.CreatedBy,
                                    VariableId = v.VariableId,
                                    CreatedDate = v.CreatedDate.ToString("dd-mm-yyyy HH:mm:ss")
                                }).ToListAsync();
            if (result == null || result.Count == 0)
            {
                return StatusCode(403);
            }

            var DeviceName = await (from vv in variableValues
                                    join v in variables on vv.VariableId equals v.VariableId
                                    join dp in devicePin on v.PinId equals dp.PinId
                                    join d in _context.Device on dp.chipId equals d.ChipId
                                    where d.ReadAPIKey == Token
                                    select d.Description).FirstOrDefaultAsync();

            return new JsonResult(new { DeviceName= DeviceName, Data = result });
        }
        //// POST: api/thethingsnetwork
        //[HttpPost("thethingsnetwork")]
        //public async Task<ActionResult<VariableValue>> ForwardVariableValue(Object value)
        //{
        //    Console.WriteLine(value.ToString());
        //    dynamic output = JsonConvert.DeserializeObject(value.ToString());
        //    Console.WriteLine(output);
        //    dynamic data = output.payload_fields.data;
        //    Console.WriteLine(data);
        //    var val = new
        //    {
        //        test = data
        //    };
        //    return new JsonResult(val);
            
        //}

        // POST: api/thethingsnetwork
        [HttpPost("thethingsnetwork")]
        public async Task<ActionResult<VariableValue>> ForwardVariableValue(Object value)
        {
            dynamic output = JsonConvert.DeserializeObject(value.ToString());
            dynamic data = output.payload_fields.data;
            int ID = Convert.ToInt32(data[0].VariableId);

            var Token = this.Request.Headers.FirstOrDefault(c => c.Key == "Token").Value.ToString();



            var WriteToken = await (from vv in _context.Variable.Where(e => e.VariableId == ID )
                                    join v in _context.Variable on vv.VariableId equals v.VariableId
                                    join dp in _context.DevicePin on v.PinId equals dp.PinId
                                    join d in _context.Device on dp.chipId equals d.ChipId
                                    select d.WriteAPIKey).SingleOrDefaultAsync();
            //if (Token != WriteToken)
            //    return StatusCode(403);

            foreach (var i in data)
            {
                var variableValue = new VariableValue();
                variableValue.VariableId = i.VariableId;
                variableValue.Value = i.Value;

                _context.VariableValue.Add(variableValue);
                _context.SaveChanges();

            }
            return NoContent();
        }

        // POST: api/VariableValues
        [HttpPost]
        public async Task<ActionResult<VariableValue>> PostVariableValue(List<VariableValue> variableValue)
        {
            var Token = this.Request.Headers.FirstOrDefault(c => c.Key == "Token").Value.ToString();

            var WriteToken = await (from vv in _context.Variable.Where(e => e.VariableId == variableValue[0].VariableId)
                                    join v in _context.Variable on vv.VariableId equals v.VariableId
                                    join dp in _context.DevicePin on v.PinId equals dp.PinId
                                    join d in _context.Device on dp.chipId equals d.ChipId
                                    select d.WriteAPIKey).SingleOrDefaultAsync();
            if (Token != WriteToken)
                return StatusCode(403);
            foreach (var v in variableValue)
            {
                _context.VariableValue.Add(v);
                _context.SaveChanges();
            }
            return NoContent();
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
