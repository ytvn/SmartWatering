using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookListMVC.Models;
using SmartWatering.Services;
using SmartWatering.Models;

namespace SmartWatering.Controllers
{
    public class TriggersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TriggersController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetInfo()
        {
            var val = _context.Variable.Select(x => new { x.VariableId, x.VariableName });
            return new JsonResult(val);
        }
     
      
        // GET: Triggers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trigger.ToListAsync());
        }

        // GET: Triggers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var trigger = await _context.Trigger
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trigger == null)
            {
                return View("NotFound");
            }

            return View(trigger);
        }

        // GET: Triggers/Create
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Control()
        {
            return View();
        }

        // POST: Triggers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,VariableId,Description,min,max,Interval,Duration")] Trigger trigger)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trigger);
                _context.SaveChanges();
                var item = from t in _context.Trigger
                           join v in _context.Variable on t.VariableId equals v.VariableId
                           join d in _context.DevicePin on v.PinId equals d.PinId
                           where t.VariableId == trigger.VariableId
                           select new { d.chipId };
                var e = SocketServer.Sockets.First(kvp =>kvp.Value== Convert.ToString(item.FirstOrDefault().chipId));
                int indexOfClient = e.Key;
                SocketServer.send(SocketServer.listClient[indexOfClient], $"T:{trigger.VariableId}:{trigger.min}:{trigger.max}");
                return RedirectToAction(nameof(Index));
            }
            return View(trigger);
        }
        // POST: Triggers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Control([Bind("Device, Status, PIN")] Control control)
        {
            if (ModelState.IsValid)
            {
                var item = SocketServer.Sockets.First(kvp => kvp.Value == control.Device);
                int indexOfClient = item.Key;
                SocketServer.send(SocketServer.listClient[indexOfClient], $"C:{control.PIN}:{control.Status}");
            }
            return View();
        }
        //void DoSchedule()
        //{
        //    var item = SocketServer.Sockets.First(kvp => kvp.Value == this.Device);
        //    int indexOfClient = item.Key;
        //    SocketServer.send(SocketServer.listClient[indexOfClient], $"{PIN}:1");

        //    Console.WriteLine("Done!");
        //}
        // GET: Triggers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var trigger = await _context.Trigger.FindAsync(id);
            if (trigger == null)
            {
                return View("NotFound");
            }
            return View(trigger);
        }

        // POST: Triggers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VariableId,Description,min,max,Interval,Duration,CreatedDate,UpdatedDate")] Trigger trigger)
        {
            if (id != trigger.Id)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trigger);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TriggerExists(trigger.Id))
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
            return View(trigger);
        }

        // GET: Triggers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var trigger = await _context.Trigger
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trigger == null)
            {
                return View("NotFound");
            }

            return View(trigger);
        }

        // POST: Triggers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trigger = await _context.Trigger.FindAsync(id);
            _context.Trigger.Remove(trigger);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TriggerExists(int id)
        {
            return _context.Trigger.Any(e => e.Id == id);
        }
    }
}
