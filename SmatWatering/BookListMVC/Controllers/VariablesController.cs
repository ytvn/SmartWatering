using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookListMVC.Models;
using SmartWatering.Models.ViewModels;

namespace SmartWatering.Controllers
{
    public class VariablesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VariablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Variables
        public async Task<IActionResult> Index()
        {
            var variable = _context.Variable;
            var devicePin = _context.DevicePIn;
            var entryPoint = from v in variable
                             join d in devicePin on v.PinId equals d.PinId
                             select  new VariableView
                             {
                                 VariableId = v.VariableId,
                                 VariableName = v.VariableName,
                                 PIN = d.PIN,
                                 ChipId = d.chipId,
                                 CreatedDate = v.CreatedDate,
                                 UpdatedDate = v.UpdatedDate
                             };
          
            return View(entryPoint.ToList());
        }
        
        // GET: Variables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var variable = _context.Variable;
            var devicePin = _context.DevicePIn;
            var variables = from v in variable
                             join d in devicePin on v.PinId equals d.PinId
                             where v.PinId==id
                             select new VariableView
                             {
                                 VariableId = v.VariableId,
                                 VariableName = v.VariableName,
                                 PIN = d.PIN,
                                 ChipId = d.chipId,
                                 CreatedDate = v.CreatedDate,
                                 UpdatedDate = v.UpdatedDate
                             };
            //var variable = await _context.Variable.ToListAsync();
            //var variables = variable.Where(m => m.PinId == id);
            if (variables == null)
            {
                return NotFound();
            }

            return View(variables);
        }

        // GET: Variables/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Variables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VariableId,VariableName,PinId")] Variable variable)
        {
            if (ModelState.IsValid)
            {
                variable.CreatedDate = DateTime.Now;
                variable.UpdatedDate = DateTime.Now;
                _context.Add(variable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(variable);
        }

        // GET: Variables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variable = await _context.Variable.FindAsync(id);
            if (variable == null)
            {
                return NotFound();
            }
            return View(variable);
        }

        // POST: Variables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VariableId,VariableName,PinId,CreatedDate,UpdatedDate")] Variable variable)
        {
            if (id != variable.VariableId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(variable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VariableExists(variable.VariableId))
                    {
                        return NotFound();
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

        // GET: Variables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variable = await _context.Variable
                .FirstOrDefaultAsync(m => m.VariableId == id);
            if (variable == null)
            {
                return NotFound();
            }

            return View(variable);
        }

        // POST: Variables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var variable = await _context.Variable.FindAsync(id);
            _context.Variable.Remove(variable);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        private bool VariableExists(int id)
        {
            return _context.Variable.Any(e => e.VariableId == id);
        }
    }
}
