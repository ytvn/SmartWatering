using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookListMVC.Models;
using SmartWatering.Models.ViewModels;
using Nancy.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using SmartWatering.Models;
using System.Security.Claims;

namespace SmartWatering.Controllers
{
    public class VariableValuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private IAuthorizationService authorizationService;

        public VariableValuesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService)
        {
            _context = context;
            this.userManager = userManager;
            this.authorizationService = authorizationService;
        }

        //api
        public async Task<IActionResult> ApiIndex()
        {
            //var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var count = _context.VariableValue.Count();
            var variableValues = from v in _context.VariableValue
                                 join e in _context.Variable on v.VariableId equals e.VariableId
                                 join d in _context.DevicePin on e.PinId equals d.PinId
                                 orderby v.VariableValueId descending
                                 select new VariableValueView
                                 {
                                     VariableValueId = v.VariableValueId,
                                     VariableName = e.VariableName,
                                     Value = v.Value,
                                     PIN = d.PIN,
                                     ChipId = d.chipId,
                                     CreatedBy = e.CreatedBy,
                                     CreatedDate = v.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss")
                                 };
            //if (!(await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
            //{
            //    variableValues = variableValues.Where(c => c.CreatedBy == LoginUserId);
            //}
            return Json(new { quantity = count, data = variableValues });
        }

        // GET: VariableValues
        [HttpGet, ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var variableValues = await (from v in _context.VariableValue
                                        join e in _context.Variable on v.VariableId equals e.VariableId
                                        join d in _context.DevicePin on e.PinId equals d.PinId
                                        select new VariableValueView
                                        {
                                            VariableValueId = v.VariableValueId,
                                            Value = v.Value,
                                            VariableName = e.VariableName,
                                            PIN = d.PIN,
                                            ChipId = d.chipId,
                                            CreatedBy = e.CreatedBy,
                                            VariableId = v.VariableId,
                                            CreatedDate = v.CreatedDate.ToString("dd-mm-yyyy HH:mm:ss")
                                        }).ToListAsync();
            if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
            {
                return View(variableValues);
            }
            return View(variableValues.Where(c => c.CreatedBy == LoginUserId).ToList());
        }

        // GET: VariableValues
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var variableValues = await (from v in _context.VariableValue
                                        join e in _context.Variable on v.VariableId equals e.VariableId
                                        join d in _context.DevicePin on e.PinId equals d.PinId
                                        select new VariableValueView
                                        {
                                            VariableValueId = v.VariableValueId,
                                            Value = v.Value,
                                            VariableName = e.VariableName,
                                            PIN = d.PIN,
                                            ChipId = d.chipId,
                                            CreatedBy = e.CreatedBy,
                                            VariableId = v.VariableId,
                                            CreatedDate = v.CreatedDate.ToString("dd-mm-yyyy HH:mm:ss")
                                        }).ToListAsync();
            if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
            {
                return View(variableValues.Where(c => c.VariableId == id).ToList());
            }
            return View(variableValues.Where(c => c.CreatedBy == LoginUserId && c.VariableId == id).ToList());
        }
        // GET: VariableValues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VariableValues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("VariableValueId,Value,VariableId,CreatedDate")] VariableValue variableValue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(variableValue);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(variableValue);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookFromDb = await _context.VariableValue.FirstOrDefaultAsync(u => u.VariableValueId == id);
            if (bookFromDb == null)
            {
                return Json(new { status = false, message = "Error while Deleting" });
            }
            _context.VariableValue.Remove(bookFromDb);
            await _context.SaveChangesAsync();
            return Json(new { status = true, message = "Delete successful" });
        }
        //[HttpDelete]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var variableValue = new VariableValue();
        //    if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
        //    {
        //        variableValue = await _context.VariableValue.FindAsync(id);
        //    }
        //    else
        //    {
        //        //variableValue = (from v in _context.VariableValue
        //        //                 join d in _context.Variable on v.VariableId equals d.VariableId
        //        //                 where d.CreatedBy == LoginUserId && v.VariableValueId == id
        //        //                 select new VariableValue
        //        //                 {
        //        //                     VariableValueId = v.VariableId,
        //        //                     Value = v.Value,
        //        //                     VariableId = v.VariableId,
        //        //                     CreatedDate = v.CreatedDate,
        //        //                     UpdatedDate = v.UpdatedDate
        //        //                 }).FirstOrDefault();
        //        variableValue = await _context.VariableValue.FirstOrDefaultAsync(u => u.VariableValueId == id);
        //    }
        //    if (variableValue == null)
        //        return Json(new { status = false, message = "Error while Deleting" });
        //    _context.VariableValue.Remove(variableValue);
        //    await _context.SaveChangesAsync();
        //    return Json(new { status = true, message = "Delete successful" });

        //}
        private bool VariableValueExists(int id)
        {
            return _context.VariableValue.Any(e => e.VariableValueId == id);
        }
    }
}
