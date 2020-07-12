using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookListMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SmartWatering.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SmartWatering.Util;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http;

namespace BookListMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private IAuthorizationService authorizationService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService)
        {
            _context = context;
            _logger = logger;
            this.userManager = userManager;
            this.authorizationService = authorizationService;
        }

        // Nhan vao ID cua device
        public async Task<IActionResult> Index(int? id)
        {
            //return View(await _context.Variable.ToListAsync());
            //Filter Pin Input only
            var devicePins = _context.DevicePin;
            var variables = from v in _context.Variable
                            join dp in devicePins on v.PinId equals dp.PinId
                            where dp.PinType == PinType.IN
                            select v;
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
            if (id == null)
            {
                if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
                {
                    return View(variables);
                }
                var x = await variables.Where(c => c.CreatedBy == LoginUserId).ToListAsync();
                return View(await variables.Where(c => c.CreatedBy == LoginUserId).ToListAsync());
            }
            else
            {
                ViewBag.chipId = id;
                if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
                {
                    return View(from v in variables
                                join dp in devicePins on v.PinId equals dp.PinId
                                where dp.chipId == id
                                select v);
                }
                return View(from v in variables
                            join dp in devicePins on v.PinId equals dp.PinId
                            where dp.chipId == id && v.CreatedBy == LoginUserId
                            select v);
            }
        }


        public async Task<IActionResult> OverView()
        {
            return View(await _context.Device.ToListAsync());
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //API
        [HttpGet]
        public async Task<IActionResult> GetVariables(int? id)
        {
            var devicePins = _context.DevicePin;
            var variables = from v in _context.Variable
                            join dp in devicePins on v.PinId equals dp.PinId
                            where dp.PinType == PinType.IN
                            select v;
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
            {
                if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
                {
                    return Json(variables);
                }
                var x = await variables.Where(c => c.CreatedBy == LoginUserId).ToListAsync();
                return Json(await variables.Where(c => c.CreatedBy == LoginUserId).ToListAsync());
            }
            else
            {
                if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
                {
                    return Json(from v in variables
                                join dp in devicePins on v.PinId equals dp.PinId
                                where dp.chipId == id
                                select v);
                }
                return Json(from v in variables
                            join dp in devicePins on v.PinId equals dp.PinId
                            where dp.chipId == id && v.CreatedBy == LoginUserId
                            select v);
                //return Json( await _context.Variable.ToListAsync());
            }
        }

        //If id != null => select all variable values, else select variable value corresponding with that chipId
        [HttpGet] 
        public async Task<IActionResult> GetVariableValues(int? id)
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var variables = _context.Variable;
            // Join to filter variable of Input Pin only
            var variableValues = from v in variables
                                 join dp in _context.DevicePin on v.PinId equals dp.PinId
                                 join vv in _context.VariableValue on v.VariableId equals vv.VariableId
                                 where dp.PinType == PinType.IN 
                                 select vv;
            if (id != null)
            {
                variableValues = from v in variables
                                 join dp in _context.DevicePin on v.PinId equals dp.PinId
                                 join vv in _context.VariableValue on v.VariableId equals vv.VariableId
                                 where dp.PinType == PinType.IN && dp.chipId == id
                                 select vv;
            }
            //Group by to select the lastest value of each Variable
            var firstItemsInGroup = variableValues.GroupBy(c => c.VariableId)
                                    .Select(s => new
                                    {
                                        VariableValueId = s.Max(c => c.VariableValueId),
                                        VariableId = s.Key,
                                    });
            if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
            {
                //Join to select Value of Variable
                return Json(from f in firstItemsInGroup
                            join v in variableValues on f.VariableId equals v.VariableId
                            where f.VariableValueId == v.VariableValueId
                            select new
                            {
                                Id = v.VariableId,
                                Value = v.Value
                            });
            }
            return Json(from f in firstItemsInGroup
                        join vv in variableValues on f.VariableId equals vv.VariableId
                        join v in variables on vv.VariableId equals v.VariableId
                        where f.VariableValueId == vv.VariableValueId && v.CreatedBy == LoginUserId
                        select new
                        {
                            Id = vv.VariableId,
                            Value = vv.Value
                        });

        }

        //If id != null => select all variable values, else select variable value corresponding with that chipId
        [HttpGet]
        public async Task<IActionResult> GetAllVariableValues(int? id)
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var variables = _context.Variable;
            // Join to filter variable of Input Pin only
            var variableValues = from v in variables
                                 join dp in _context.DevicePin on v.PinId equals dp.PinId
                                 join vv in _context.VariableValue on v.VariableId equals vv.VariableId
                                 where dp.PinType == PinType.IN
                                 select vv;
            if (id != null)
            {
                variableValues = from v in variables
                                 join dp in _context.DevicePin on v.PinId equals dp.PinId
                                 join vv in _context.VariableValue on v.VariableId equals vv.VariableId
                                 where dp.PinType == PinType.IN && dp.chipId == id
                                 select vv;
            }
            
            if (!(await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
            {
                variableValues = from vv in variableValues
                                 join v in variables on vv.VariableId equals v.VariableId
                                 where v.CreatedBy == LoginUserId
                                 select vv;
            }
            return Json(from vv in variableValues
                        join v in variables on vv.VariableId equals v.VariableId
                        select new 
                        { 
                            Id = vv.VariableValueId,
                            Name = v.VariableName,
                            Value = vv.Value,
                            CreatedDate = vv.CreatedDate
                        });

        }
        
        [HttpGet]
        public async Task<IActionResult> ExportAllVariableValues(int? id)
        {
            int i = 2;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var variables = _context.Variable;
            // Join to filter variable of Input Pin only
            var variableValues = from v in variables
                                 join dp in _context.DevicePin on v.PinId equals dp.PinId
                                 join vv in _context.VariableValue on v.VariableId equals vv.VariableId
                                 where dp.PinType == PinType.IN
                                 select vv;
            if (id != null)
            {
                variableValues = from v in variables
                                 join dp in _context.DevicePin on v.PinId equals dp.PinId
                                 join vv in _context.VariableValue on v.VariableId equals vv.VariableId
                                 where dp.PinType == PinType.IN && dp.chipId == id
                                 select vv;
            }
            
            if (!(await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
            {
                variableValues = from vv in variableValues
                                 join v in variables on vv.VariableId equals v.VariableId
                                 where v.CreatedBy == LoginUserId
                                 select vv;
            }
            ws.Cells["A1"].Value = "ID";
            ws.Cells["B1"].Value = "Name";
            ws.Cells["C1"].Value = "Value";
            ws.Cells["D1"].Value = "CreatedDate";
            var result = (from vv in variableValues
                          join v in variables on vv.VariableId equals v.VariableId
                          select new
                          {
                              Id = vv.VariableValueId,
                              Name = v.VariableName,
                              Value = vv.Value,
                              CreatedDate = vv.CreatedDate
                          }).ToList();
            foreach (var item in result)
            {
                ws.Cells[string.Format("A{0}", i)].Value = item.Id;
                ws.Cells[string.Format("B{0}", i)].Value = item.Name;
                ws.Cells[string.Format("C{0}", i)].Value = item.Value;
                ws.Cells[string.Format("D{0}", i)].Value = item.CreatedDate;
                i++;
            }
            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Headers.Add("content-disposition", "attachment: filename=" + "ExcelReport.xlsx");
            await Response.Body.WriteAsync(pck.GetAsByteArray());
            return Json(result);

        }
        //http://localhost:5005/home/average/?type=2&deviceId=3&variableId=22
        [HttpGet]
        public async Task<IActionResult> Average(int type, int? deviceId, int variableId)
        {
            if (deviceId == null)
                return Json(new { });

            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var variables = await _context.Variable.Where(c => c.VariableId == variableId).ToListAsync();

            // Join to filter variable of Input Pin only and value only for 1 device
            var variableValues = from v in variables
                                 join dp in _context.DevicePin on v.PinId equals dp.PinId
                                 join vv in _context.VariableValue on v.VariableId equals vv.VariableId
                                 where dp.PinType == PinType.IN && dp.chipId == deviceId
                                 select vv;
            if (!(await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
            {
                variableValues = from vv in variableValues
                                 join v in variables on vv.VariableId equals v.VariableId
                                 where v.CreatedBy == LoginUserId
                                 select vv;
            }

            if (type == 1)
                variableValues = variableValues.Where(c => DateTime.Now.Day == c.CreatedDate.Day);
            else if (type == 2)
                variableValues = variableValues.Where(c => IsInWeek(c.CreatedDate.Day));
            else if (type == 3)
                variableValues = variableValues.Where(c => DateTime.Now.Month == c.CreatedDate.Month);

            var result = (variableValues
                                .GroupBy(x => x.VariableId)
                                .Select(y => new { VariableId = y.Key, Average = Math.Round(y.Average(x => x.Value), 2) }))
                                .FirstOrDefault();
            if (result == null)
                return Json(new { VariableId=-1, Average=-1 });
            return new JsonResult(result );
        }
        private bool IsInWeek(int day)
        {
            var now = ((int)DateTime.Now.DayOfWeek + 6) % 7 +1;
            var begin = DateTime.Now.Day - (now -1);
            var end = DateTime.Now.Day +(7-now);
            if (day >= begin && day <= end)
                return true;
            return false;
        }
    }
}
