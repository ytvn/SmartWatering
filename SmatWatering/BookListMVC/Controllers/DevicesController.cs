using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookListMVC.Models;
using SmartWatering.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SmartWatering.Models;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;


namespace SmartWatering.Controllers
{
    [Authorize]
    public class DevicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private IAuthorizationService authorizationService;


        public DevicesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService)
        {
            _context = context;
            this.userManager = userManager;
            this.authorizationService = authorizationService;
        }
        public IActionResult GetInfo()
        {
            var val = _context.Device.Select(x => new { x.Description, x.ChipId });
            return new JsonResult(val);
        }
        // GET: Devices
       
        public IActionResult ApiIndex()
        {
            var count = _context.Device.Count();
            var devices = from v in _context.Device
                          orderby v.CreatedDate descending
                          select new DeviceView
                          {
                              ChipId = v.ChipId,
                              Description = v.Description
                          };
            return Json(new { quantity = count, data = devices });
        }

        public async Task<IActionResult> Index()
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
            {
                return View(await _context.Device.ToListAsync());
            }
            return View(await _context.Device.Where(c => c.CreatedBy == LoginUserId).ToListAsync());
        }
       

        // GET: Devices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Devices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Create([Bind("DeviceId,ChipId,Description")] Device device)
        {

            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (LoginUserId == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {LoginUserId} cannot be found";
                return View("NotFound");
            }
            device.CreatedBy = LoginUserId;
            device.UpdatedBy = LoginUserId;
            if (ModelState.IsValid)
            {
                _context.Add(device);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(device);
        }

        // GET: Devices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var device = await _context.Device.FindAsync(id);
            
            if (device == null)
            {
                return View("NotFound");
            }
            if(device.CreatedBy!=LoginUserId && !(await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
            {
                return View("AccessDenied");
            }
            return View(device);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DeviceId,ChipId,Description")] Device device)
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != device.DeviceId)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    device.UpdatedBy = LoginUserId;
                    _context.Update(device);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceExists(device.DeviceId))
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
            return View(device);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var LoginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var device = new Device();
            if ((await authorizationService.AuthorizeAsync(User, "AdminPolicy")).Succeeded)
            {
                device = await _context.Device.FindAsync(id);
            }
            else
            {
                device = await _context.Device.Where(c => c.CreatedBy == LoginUserId && c.DeviceId == id)
                    .FirstOrDefaultAsync();
            }
            if (device == null)
                return Json(new { status = false, message = "Error while Deleting" });
            _context.Device.Remove(device);
            await _context.SaveChangesAsync();
            return Json(new { status = true, message = "Delete successful" });
            
        }
        private bool DeviceExists(int id)
        {
            return _context.Device.Any(e => e.DeviceId == id);
        }

    }
}
