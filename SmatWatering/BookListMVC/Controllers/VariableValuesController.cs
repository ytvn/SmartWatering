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

namespace SmartWatering.Controllers
{
    public class VariableValuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VariableValuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VariableValues
        public async Task<IActionResult> Index()
        {
            var variableValues = from v in _context.VariableValue
                                 join e in _context.Variable on v.VariableId equals e.VariableId
                                 join d in _context.DevicePIn on e.PinId equals d.PinId
                                 select new VariableValueView
                                 {
                                     VariableValueId = v.VariableValueId,
                                     Value = v.Value,
                                     VariableName = e.VariableName,
                                     PIN = d.PIN,
                                     ChipId = d.chipId,
                                     CreatedDate = v.CreatedDate.ToString("dd-mm-yyyy HH:mm:ss")
                                 };
            return View(variableValues);
        }
        //api
        public async Task<IActionResult> ApiIndex()
        {
            var count = _context.VariableValue.Count();
            var variableValues = from v in _context.VariableValue
                                 join e in _context.Variable on v.VariableId equals e.VariableId
                                 join d in _context.DevicePIn on e.PinId equals d.PinId
                                 orderby v.VariableValueId descending
                                 select new VariableValueView
                                 {
                                     VariableValueId = v.VariableValueId,
                                     VariableName = e.VariableName,
                                     Value = v.Value,
                                     PIN = d.PIN,
                                     ChipId = d.chipId,
                                     CreatedDate = v.CreatedDate.ToString("dd-mm-yyyy HH:mm:ss")
                                 };

            return Json(new { quantity = count, data = variableValues });
        }

        // GET: VariableValues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var variableValues = from v in _context.VariableValue
                                 join e in _context.Variable on v.VariableId equals e.VariableId
                                 join d in _context.DevicePIn on e.PinId equals d.PinId
                                 where v.VariableId == id
                                 select new VariableValueView
                                 {
                                     VariableValueId = v.VariableValueId,
                                     Value = v.Value,
                                     VariableName = e.VariableName,
                                     PIN = d.PIN,
                                     ChipId = d.chipId,
                                     CreatedDate = v.CreatedDate.ToString("dd-mm-yyyy HH:mm:ss")
                                 };
            if (variableValues == null)
            {
                return NotFound();
            }

            return View(variableValues);
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
        public async Task<IActionResult> Create([Bind("VariableValueId,Value,VariableId,CreatedDate")] VariableValue variableValue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(variableValue);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(variableValue);
        }

        // GET: VariableValues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variableValue = await _context.VariableValue.FindAsync(id);
            if (variableValue == null)
            {
                return NotFound();
            }
            return View(variableValue);
        }

        // POST: VariableValues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VariableValueId,Value,VariableId,CreatedDate")] VariableValue variableValue)
        {
            if (id != variableValue.VariableValueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(variableValue);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VariableValueExists(variableValue.VariableValueId))
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
            return View(variableValue);
        }

        // GET: VariableValues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variableValue = await _context.VariableValue
                .FirstOrDefaultAsync(m => m.VariableValueId == id);
            if (variableValue == null)
            {
                return NotFound();
            }

            return View(variableValue);
        }

        // POST: VariableValues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var variableValue = await _context.VariableValue.FindAsync(id);
            _context.VariableValue.Remove(variableValue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookFromDb = await _context.VariableValue.FirstOrDefaultAsync(u => u.VariableValueId == id);
            if (bookFromDb == null)
            {
                return Json(new { status = false, message = "Error while Deleting" });
            }
            _context.VariableValue.Remove(bookFromDb);
            await _context.SaveChangesAsync();
            return Json(new { status = true, message = "Delete successful" });
        }


        private bool VariableValueExists(int id)
        {
            return _context.VariableValue.Any(e => e.VariableValueId == id);
        }
    }
}
