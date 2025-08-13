using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;

namespace AssetManagement.Controllers
{
    public class LocationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var locations = await _context.Locations
                .Include(l => l.FloorPlans)
                .Where(l => l.IsActive)
                .ToListAsync();

            return View(locations);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Address,City,State,ZipCode,Notes")] Location location)
        {
            if (ModelState.IsValid)
            {
                location.IsActive = true;
                location.CreatedAt = DateTime.UtcNow;
                _context.Add(location);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Location created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,City,State,ZipCode,Notes,IsActive")] Location location)
        {
            if (id != location.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    location.UpdatedAt = DateTime.UtcNow;
                    _context.Update(location);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Location updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.Id))
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
            return View(location);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .Include(l => l.FloorPlans)
                    .ThenInclude(f => f.Desks)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            location.IsActive = false;
            location.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.Id == id);
        }
    }
}
