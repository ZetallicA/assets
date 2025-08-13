using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;

namespace AssetManagement.Controllers
{
    public class EntraUserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EntraUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.EntraUsers
                .Include(e => e.AssignedEquipment)
                .Where(e => e.IsActive)
                .ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.EntraUsers
                                    .Include(e => e.AssignedEquipment)
                        .ThenInclude(eq => eq.AssetCategory)
                    .Include(e => e.AssignedEquipment)
                        .ThenInclude(eq => eq.CurrentStatus)
                .Include(e => e.AssignedEquipment)
                    .ThenInclude(eq => eq.CurrentLocation)
                .Include(e => e.AssignedEquipment)
                    .ThenInclude(eq => eq.CurrentFloorPlan)
                .Include(e => e.AssignedEquipment)
                    .ThenInclude(eq => eq.CurrentDesk)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.EntraUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ObjectId,DisplayName,UserPrincipalName,GivenName,Surname,JobTitle,Department,OfficeLocation,IsActive")] EntraUser user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.UpdatedAt = DateTime.UtcNow;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "User updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntraUserExists(user.Id))
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
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.EntraUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UnassignEquipment(int userId, int equipmentId)
        {
            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(e => e.Id == equipmentId && e.AssignedEntraUserId == userId);

            if (equipment == null)
            {
                return NotFound();
            }

            equipment.AssignedEntraUserId = null;
            equipment.AssignedEntraObjectId = null;
                            equipment.UpdatedAt = DateTime.UtcNow;

                // Get user details for logging
                var user = await _context.EntraUsers.FindAsync(userId);

                // Log the unassignment
                var auditLog = new AssetAuditLog
            {
                EquipmentId = equipmentId,
                Action = "Unassigned from User",
                Details = $"Unassigned from user {user?.DisplayName ?? "Unknown"}",
                PerformedAt = DateTime.UtcNow,
                PerformedBy = "System" // TODO: Get from authentication
            };
            _context.AssetAuditLogs.Add(auditLog);

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new List<object>());
            }

            var users = await _context.EntraUsers
                .Where(e => e.IsActive && 
                           (e.DisplayName.Contains(query) || 
                            e.UserPrincipalName.Contains(query) || 
                            e.GivenName.Contains(query) || 
                            e.Surname.Contains(query)))
                .Select(e => new
                {
                    id = e.Id,
                    displayName = e.DisplayName,
                    userPrincipalName = e.UserPrincipalName,
                    jobTitle = e.JobTitle,
                    department = e.Department
                })
                .Take(10)
                .ToListAsync();

            return Json(users);
        }

        private bool EntraUserExists(int id)
        {
            return _context.EntraUsers.Any(e => e.Id == id);
        }
    }
}
