using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using AssetManagement.Data;
using AssetManagement.Models;

namespace AssetManagement.Controllers
{
    [Route("[controller]")]
    public class FloorPlanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FloorPlanController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var floorPlans = await _context.FloorPlans
                .Include(f => f.Location)
                .Include(f => f.Desks)
                .Where(f => f.IsActive)
                .ToListAsync();

            // Debug: Log all floor plan IDs
            Console.WriteLine("Available floor plans:");
            foreach (var fp in floorPlans)
            {
                Console.WriteLine($"ID: {fp.Id}, Location: {fp.Location?.Name}, Floor: {fp.FloorNumber}");
            }

            return View(floorPlans);
        }

        public async Task<IActionResult> Create()
        {
            var locations = await _context.Locations
                .Where(l => l.IsActive)
                .ToListAsync();
            ViewBag.Locations = new SelectList(locations, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationId,FloorNumber,FloorName,Description")] FloorPlan floorPlan, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "floorplans");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    floorPlan.ImagePath = "/uploads/floorplans/" + fileName;
                }

                floorPlan.IsActive = true;
                floorPlan.CreatedAt = DateTime.UtcNow;
                _context.Add(floorPlan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Floor plan created successfully.";
                return RedirectToAction(nameof(Index));
            }

            var locations = await _context.Locations
                .Where(l => l.IsActive)
                .ToListAsync();
            ViewBag.Locations = new SelectList(locations, "Id", "Name");
            return View(floorPlan);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Console.WriteLine($"Attempting to edit floor plan with ID: {id}");
            
            var floorPlan = await _context.FloorPlans.FindAsync(id);
            if (floorPlan == null)
            {
                Console.WriteLine($"Floor plan with ID {id} not found");
                return NotFound();
            }

            Console.WriteLine($"Found floor plan: {floorPlan.FloorNumber} at location {floorPlan.LocationId}");

            var locations = await _context.Locations
                .Where(l => l.IsActive)
                .ToListAsync();
            
            ViewBag.Locations = new SelectList(locations, "Id", "Name");
            return View(floorPlan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LocationId,FloorNumber,FloorName,Description,IsActive")] FloorPlan floorPlan, IFormFile? imageFile)
        {
            if (id != floorPlan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "floorplans");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        floorPlan.ImagePath = "/uploads/floorplans/" + fileName;
                    }

                    floorPlan.UpdatedAt = DateTime.UtcNow;
                    _context.Update(floorPlan);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Floor plan updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FloorPlanExists(floorPlan.Id))
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

            var locations = await _context.Locations
                .Where(l => l.IsActive)
                .ToListAsync();
            ViewBag.Locations = new SelectList(locations, "Id", "Name");
            return View(floorPlan);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var floorPlan = await _context.FloorPlans
                .Include(f => f.Location)
                .Include(f => f.Desks)
                    .ThenInclude(d => d.Equipment)
                        .ThenInclude(e => e.AssignedEntraUser)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (floorPlan == null)
            {
                return NotFound();
            }

            return View(floorPlan);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var floorPlan = await _context.FloorPlans.FindAsync(id);
            if (floorPlan == null)
            {
                return NotFound();
            }

            floorPlan.IsActive = false;
            floorPlan.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AddDesk(int floorPlanId, string deskNumber, string deskName, int x, int y)
        {
            var desk = new Desk
            {
                FloorPlanId = floorPlanId,
                DeskNumber = deskNumber,
                DeskName = deskName,
                XCoordinate = x,
                YCoordinate = y,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Desks.Add(desk);
            await _context.SaveChangesAsync();

            return Json(new { success = true, deskId = desk.Id });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDesk(int deskId, string deskNumber, string deskName, int x, int y)
        {
            var desk = await _context.Desks.FindAsync(deskId);
            if (desk == null)
            {
                return NotFound();
            }

            desk.DeskNumber = deskNumber;
            desk.DeskName = deskName;
            desk.XCoordinate = x;
            desk.YCoordinate = y;
            desk.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDesk(int deskId)
        {
            var desk = await _context.Desks.FindAsync(deskId);
            if (desk == null)
            {
                return NotFound();
            }

            desk.IsActive = false;
            desk.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet("GetFloorPlansByLocation/{locationId}")]
        public async Task<IActionResult> GetFloorPlansByLocation(int locationId)
        {
            // Debug: Log the location being requested with stack trace to see WHO called this
            var stackTrace = new System.Diagnostics.StackTrace();
            Console.WriteLine($"=== API CALL DEBUG ===");
            Console.WriteLine($"Requested floor plans for location ID: {locationId}");
            Console.WriteLine($"Called at: {DateTime.Now:HH:mm:ss.fff}");
            Console.WriteLine($"Request URL: {Request.Path}{Request.QueryString}");
            Console.WriteLine($"========================");
            
            // First, check if the location exists
            var location = await _context.Locations.FindAsync(locationId);
            Console.WriteLine($"Location found: {location?.Name ?? "NULL"}");
            
            // Get all floor plans for this location (including inactive ones for debugging)
            var allFloorPlans = await _context.FloorPlans
                .Where(f => f.LocationId == locationId)
                .Select(f => new
                {
                    id = f.Id,
                    floorNumber = f.FloorNumber,
                    floorName = f.FloorName,
                    isActive = f.IsActive
                })
                .ToListAsync();
            
            Console.WriteLine($"All floor plans for location {locationId}: {string.Join(", ", allFloorPlans.Select(f => $"{f.floorNumber} (Active: {f.isActive})"))}");
            
            // Return only active floor plans
            var activeFloorPlans = allFloorPlans.Where(f => f.isActive).Select(f => new
            {
                id = f.id,
                floorNumber = f.floorNumber,
                floorName = f.floorName
            }).ToList();
            
            Console.WriteLine($"Active floor plans returned: {activeFloorPlans.Count}");
            Console.WriteLine($"=== END API CALL ===");

            return Json(activeFloorPlans);
        }

        [HttpGet("GetDesksByFloorPlan/{floorPlanId}")]
        public async Task<IActionResult> GetDesksByFloorPlan(int floorPlanId)
        {
            var desks = await _context.Desks
                .Where(d => d.FloorPlanId == floorPlanId && d.IsActive)
                .Select(d => new
                {
                    id = d.Id,
                    deskNumber = d.DeskNumber,
                    deskName = d.DeskName
                })
                .ToListAsync();

            return Json(desks);
        }

        private bool FloorPlanExists(int id)
        {
            return _context.FloorPlans.Any(e => e.Id == id);
        }
    }
}
