using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookListMVC.Models;
using SmartWatering.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using SmartWatering.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SmartWatering.Controllers
{
    public class VariablesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private IAuthorizationService authorizationService;

        public VariablesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService)
        {
            _context = context;
            this.userManager = userManager;
            this.authorizationService = authorizationService;
        }

        // GET: Variables
        public async Task<IActionResult> Index(int? id)
        {
            var variable = _context.Variable;
            var devicePin = _context.DevicePin;
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var variables = (from v in variable
                             join d in devicePin on v.PinId equals d.PinId
                             select new VariableView
                             {
                                 VariableId = v.VariableId,
                                 VariableName = v.VariableName,
                                 PIN = d.PIN,
                                 ChipId = d.chipId,
                                 CreatedDate = v.CreatedDate,
                                 UpdatedDate = v.UpdatedDate,
                                 CreatedBy = v.CreatedBy,
                                 PinId = v.PinId
                             }).ToList();
            if( id == null)
            {
                if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
                {
                    return View(variables);
                }
                return View(variables.Where(c => c.CreatedBy == LoginUserId).ToList());
            }
            else
            {
                if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
                {
                    return View(variables.Where(c => c.PinId==id).ToList());
                }
                return View(variables.Where(c => c.CreatedBy == LoginUserId && c.PinId == id).ToList());
            }
           
        }

        // GET: Variables/Create
        public async Task <IActionResult> Create()
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var x = await _context.DevicePin.Where(c => c.CreatedBy == LoginUserId).ToListAsync();
            ViewBag.Values = (List<DevicePin>) await _context.DevicePin.Where(c => c.CreatedBy == LoginUserId).ToListAsync();
            return View();
        }

        // POST: Variables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Create(VariableView model)
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Variable variable = new Variable();
            if (ModelState.IsValid)
            {
                variable.VariableName = model.VariableName;
                variable.PinId = _context.DevicePin.Where( c => c.chipId==model.ChipId && c.PIN==model.PIN).FirstOrDefault().PinId;
                variable.CreatedBy = LoginUserId;
                variable.UpdatedBy = LoginUserId;
                _context.Add(variable);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(variable);
        }

        // GET: Variables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var variable = (from v in _context.Variable
                           join d in _context.DevicePin on v.PinId equals d.PinId
                           where v.VariableId == id && v.CreatedBy == LoginUserId
                           select new VariableView
                           {
                               VariableId = v.VariableId,
                               VariableName = v.VariableName,
                               PIN = d.PIN,
                               ChipId = d.chipId,
                               CreatedDate = v.CreatedDate,
                               UpdatedDate = v.UpdatedDate,
                               CreatedBy = v.CreatedBy
                           }).Single();



            if (variable == null)
            {
                return View("NotFound");
            }
          
            ViewBag.Values = (List<DevicePin>)await _context.DevicePin.Where(c => c.CreatedBy == LoginUserId).ToListAsync();
            return View(variable);
        }

        // POST: Variables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VariableId,VariableName,PinId,CreatedDate,UpdatedDate")] Variable variable)
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != variable.VariableId)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    variable.UpdatedBy = LoginUserId;
                    _context.Update(variable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VariableExists(variable.VariableId))
                    {
                        return View("NotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(variable);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var variable = new Variable();
            if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
            {
                variable = await _context.Variable.FindAsync(id);
            }
            else
            {
                variable = await _context.Variable.Where(c => c.CreatedBy == LoginUserId && c.VariableId == id)
                    .FirstOrDefaultAsync();
            }
            if (variable == null)
                return Json(new { status = false, message = "Error while Deleting" });
            _context.Variable.Remove(variable);
            await _context.SaveChangesAsync();
            return Json(new { status = true, message = "Delete successful" });

        }
        private bool VariableExists(int id)
        {
            return _context.Variable.Any(e => e.VariableId == id);
        }
    }
}
