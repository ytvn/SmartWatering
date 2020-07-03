using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookListMVC.Models;

namespace SmartWatering.Controllers
{
    public class DevicePinsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DevicePinsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetInfo(int _chipId)
        {
            var val = _context.DevicePIn.Where(x => x.chipId == _chipId).Select(x => new { value = x.PIN +": "+x.Description, x.PIN });
            return new JsonResult(val);
        }

        // GET: DevicePins
        public async Task<IActionResult> Index()
        {
            return View(await _context.DevicePIn.ToListAsync());
        }

        // GET: DevicePins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var devicePin = await _context.DevicePIn
                .ToListAsync();
            var devicePins = devicePin.Where(m => m.chipId == id);
            if (devicePins == null)
            {
                return NotFound();
            }

            return View(devicePins);
        }

        // GET: DevicePins/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: DevicePins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PinId,PIN,chipId,Description")] DevicePin devicePin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(devicePin);
              _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(devicePin);
        }

        // GET: DevicePins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var devicePin = await _context.DevicePIn.FindAsync(id);
            if (devicePin == null)
            {
                return NotFound();
            }
            return View(devicePin);
        }

        // POST: DevicePins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PinId,PIN,chipId,Description,CreatedDate,UpdatedDate")] DevicePin devicePin)
        {
            if (id != devicePin.PinId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(devicePin);
                  _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DevicePinExists(devicePin.PinId))
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
            return View(devicePin);
        }

        // GET: DevicePins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var devicePin = await _context.DevicePIn
                .FirstOrDefaultAsync(m => m.PinId == id);
            if (devicePin == null)
            {
                return NotFound();
            }

            return View(devicePin);
        }

        // POST: DevicePins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var devicePin = await _context.DevicePIn.FindAsync(id);
            _context.DevicePIn.Remove(devicePin);
          _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool DevicePinExists(int id)
        {
            return _context.DevicePIn.Any(e => e.PinId == id);
        }
    }
}
