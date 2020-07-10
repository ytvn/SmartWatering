using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookListMVC.Models;
using SmartWatering.Models;
using SmartWatering.Services;

namespace SmartWatering.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SchedulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Schedules
        public async Task<IActionResult> Index()
        {
            return View(await _context.Schedule.ToListAsync());
        }

        // GET: Schedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var schedule = await _context.Schedule
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return View("NotFound");
            }

            return View(schedule);
        }

        public IActionResult Reset()
        {
            MyScheduler.Reset();
            return NoContent();
        }

        // GET: Schedules/Create
        public IActionResult Create()
        {
            return View();
        }
        string Device, PIN;
        // POST: Schedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,intervalType,TimeStart,Interval,Device,PIN,Variable")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("Scheduled!");
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                MyScheduler.Reset();
                this.Device = schedule.Device;
                this.PIN = schedule.PIN;
                MyScheduler.Interval(schedule.TimeStart.Hours, schedule.TimeStart.Minutes, schedule.intervalType, schedule.Interval, true, DoSchedule);
                //Console.WriteLine($"{schedule.intervalType}\n{schedule.TimeStart}\n{schedule.Interval}\n{schedule.Device}\n{schedule.PIN}");

                return View();
            }
            return View(schedule);
        }
        void DoSchedule()
        {
            if (SocketServer.listClient.Count <= 0)
            {
                Console.WriteLine("Not found device");
                return;
            }
            Console.WriteLine("Done!");
            var item = SocketServer.Sockets.First(kvp => kvp.Value == this.Device);
            int indexOfClient = item.Key;
            SocketServer.send(SocketServer.listClient[indexOfClient], $"S:{PIN}:1");

            
        }

        // GET: Schedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var schedule = await _context.Schedule.FindAsync(id);
            if (schedule == null)
            {
                return View("NotFound");
            }
            return View(schedule);
        }

        // POST: Schedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,intervalType,TimeStart,Interval,Device,PIN,Variable")] Schedule schedule)
        {
            if (id != schedule.Id)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.Id))
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
            return View(schedule);
        }

        // GET: Schedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var schedule = await _context.Schedule
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return View("NotFound");
            }

            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedule.FindAsync(id);
            _context.Schedule.Remove(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedule.Any(e => e.Id == id);
        }
    }
}
