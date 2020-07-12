using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookListMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using SmartWatering.Models;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace SmartWatering.Controllers
{
    public class DevicePinsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private IAuthorizationService authorizationService;
        public DevicePinsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService)
        {
            _context = context;
            this.userManager = userManager;
            this.authorizationService = authorizationService;
        }

        [HttpGet]
        public async  Task<IActionResult> GetInfo(int _chipId)
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
            {
                return new JsonResult(_context.DevicePin
                            .Where(x => x.chipId == _chipId && x.PinType == Util.PinType.OUT)
                            .Select(x => new { value = x.PIN + ": " + x.Description, x.PIN }));
            }
            return new JsonResult(_context.DevicePin
                         .Where(x => x.chipId == _chipId && x.CreatedBy==LoginUserId && x.PinType == Util.PinType.OUT)
                         .Select(x => new { value = x.PIN + ": " + x.Description, x.PIN }));
        }

        // GET: DevicePins
        public async Task<IActionResult> Index(int?id )
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
            {
                if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
                {
                    return View(await _context.DevicePin.ToListAsync());
                }
                return View(await _context.DevicePin.Where(c => c.CreatedBy == LoginUserId).ToListAsync());
            }
            else
            {
                if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
                {
                    return View(await _context.DevicePin.Where(m => m.chipId == id).ToListAsync());
                }
                return View(await _context.DevicePin.Where(c => c.CreatedBy == LoginUserId && c.chipId==id).ToListAsync());
            }
        }

        // GET: DevicePins/Create
        public IActionResult Create()
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var value = from c in _context.Device
                        where c.CreatedBy == LoginUserId
                        select c.ChipId;
            ViewBag.Chips=  value;

            return View();
        }

        // POST: DevicePins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("PinType, PinId,PIN,chipId,Description")] DevicePin devicePin)
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            devicePin.CreatedBy = LoginUserId;
            devicePin.UpdatedBy = LoginUserId;
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
                return View("NotFound");
            }

            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var devicePin = await _context.DevicePin.FindAsync(id);

            if (devicePin == null)
            {
                return View("NotFound");
            }
            if (devicePin.CreatedBy != LoginUserId && !(await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
            {
                return View("AccessDenied");
            }

            var value = from c in _context.Device
                        where c.CreatedBy == LoginUserId
                        select c.ChipId;
            ViewBag.Chips = value;
            return View(devicePin);
        }

        // POST: DevicePins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("PinId, PinType, PIN,chipId,Description,CreatedDate,UpdatedDate")] DevicePin devicePin)
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id != devicePin.PinId)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    devicePin.UpdatedBy = LoginUserId;
                    _context.Update(devicePin);
                  _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DevicePinExists(devicePin.PinId))
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
            return View(devicePin);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var devicePin = new DevicePin();
            if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
            {
                devicePin = await _context.DevicePin.FindAsync(id);
            }
            else
            {
                devicePin = await _context.DevicePin.Where(c => c.CreatedBy == LoginUserId && c.PinId == id)
                    .FirstOrDefaultAsync();
            }
            if (devicePin == null)
                return Json(new { status = false, message = "Error while Deleting" });
            _context.DevicePin.Remove(devicePin);
            await _context.SaveChangesAsync();
            return Json(new { status = true, message = "Delete successful" });

        }

        private bool DevicePinExists(int id)
        {
            return _context.DevicePin.Any(e => e.PinId == id);
        }
    }
}
