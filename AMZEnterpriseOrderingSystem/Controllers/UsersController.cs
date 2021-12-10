using AMZEnterpriseOrderingSystem.Data;
using AMZEnterpriseOrderingSystem.Models;
using AMZEnterpriseOrderingSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AMZEnterpriseOrderingSystem.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            return View(await _context.ApplicationUser.Where(u => u.Id != claim.Value).ToListAsync());
        }



        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }
            return View(applicationUser);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userFromDb = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

                userFromDb.FirstName = model.FirstName;
                userFromDb.LastName = model.LastName;
                userFromDb.PhoneNumber = model.PhoneNumber;
                userFromDb.LockoutEnd = model.LockoutEnd;
                userFromDb.LockoutEnabled = model.LockoutEnabled;
                userFromDb.AccessFailedCount = model.AccessFailedCount;
                userFromDb.LockoutReason = model.LockoutReason;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string id, ApplicationUser model)
        {
            var userFromDb = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

            userFromDb.FirstName = model.FirstName;
            userFromDb.LastName = model.LastName;
            userFromDb.PhoneNumber = model.PhoneNumber;
            userFromDb.LockoutEnd = DateTime.Now.AddYears(100);
            userFromDb.LockoutEnabled = model.LockoutEnabled;
            userFromDb.AccessFailedCount = model.AccessFailedCount;
            userFromDb.LockoutReason = model.LockoutReason;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
