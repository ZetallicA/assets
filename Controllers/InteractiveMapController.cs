using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;

namespace AssetManagement.Controllers
{
    public class InteractiveMapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InteractiveMapController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var locations = await _context.Locations
                .Include(l => l.FloorPlans)
                    .ThenInclude(f => f.Desks)
                .Where(l => l.IsActive)
                .ToListAsync();

            return View(locations);
        }

        public async Task<IActionResult> FloorPlan(int id)
        {
            var floorPlan = await _context.FloorPlans
                .Include(f => f.Location)
                .Include(f => f.Desks)
                    .ThenInclude(d => d.Equipment)
                        .ThenInclude(e => e.AssignedEntraUser)
                .Include(f => f.Desks)
                    .ThenInclude(d => d.Equipment)
                        .ThenInclude(e => e.AssetCategory)
                .Include(f => f.Desks)
                    .ThenInclude(d => d.Equipment)
                        .ThenInclude(e => e.CurrentStatus)
                .FirstOrDefaultAsync(f => f.Id == id && f.IsActive);

            if (floorPlan == null)
            {
                return NotFound();
            }

            return View(floorPlan);
        }

        [HttpGet]
        public async Task<IActionResult> GetFloorPlanData(int id)
        {
            var floorPlan = await _context.FloorPlans
                .Include(f => f.Desks)
                    .ThenInclude(d => d.Equipment)
                        .ThenInclude(e => e.AssignedEntraUser)
                .Include(f => f.Desks)
                    .ThenInclude(d => d.Equipment)
                        .ThenInclude(e => e.AssetCategory)
                .Include(f => f.Desks)
                    .ThenInclude(d => d.Equipment)
                        .ThenInclude(e => e.CurrentStatus)
                .FirstOrDefaultAsync(f => f.Id == id && f.IsActive);

            if (floorPlan == null)
            {
                return NotFound();
            }

            var desks = floorPlan.Desks
                .Where(d => d.IsActive)
                .Select(d => new
                {
                    id = d.Id,
                    deskNumber = d.DeskNumber,
                    deskName = d.DeskName,
                    x = d.XCoordinate,
                    y = d.YCoordinate,
                    equipment = d.Equipment
                        .Where(e => e.IsActive)
                        .Select(e => new
                        {
                            id = e.Id,
                            oathTag = e.OATH_Tag,
                            assetTag = e.Asset_Tag,
                            model = e.Model,
                            manufacturer = e.Manufacturer,
                            category = e.AssetCategory?.Name,
                            status = e.CurrentStatus?.Name,
                            statusColor = e.CurrentStatus?.Color,
                            assignedUser = e.AssignedEntraUser?.DisplayName,
                            assignedEmail = e.AssignedEntraUser?.UserPrincipalName
                        }).ToList()
                }).ToList();

            return Json(new
            {
                floorPlan = new
                {
                    id = floorPlan.Id,
                    floorNumber = floorPlan.FloorNumber,
                    floorName = floorPlan.FloorName,
                    description = floorPlan.Description,
                    imagePath = floorPlan.ImagePath
                },
                desks = desks
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDeskPosition(int deskId, int x, int y)
        {
            var desk = await _context.Desks.FindAsync(deskId);
            if (desk == null)
            {
                return NotFound();
            }

            desk.XCoordinate = x;
            desk.YCoordinate = y;
            desk.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AssignEquipmentToDesk(int equipmentId, int deskId)
        {
            var equipment = await _context.Equipment.FindAsync(equipmentId);
            if (equipment == null)
            {
                return NotFound();
            }

            var desk = await _context.Desks
                .Include(d => d.FloorPlan)
                .FirstOrDefaultAsync(d => d.Id == deskId);
            if (desk == null)
            {
                return NotFound();
            }

            equipment.CurrentDeskId = deskId;
            equipment.CurrentFloorPlanId = desk.FloorPlanId;
            equipment.CurrentLocationId = desk.FloorPlan.LocationId;
            equipment.UpdatedAt = DateTime.UtcNow;

                            // Log the assignment
                var auditLog = new AssetAuditLog
                {
                    EquipmentId = equipmentId,
                    Action = "Assigned to Desk",
                    Details = $"Assigned to desk {desk.DeskNumber} on floor {desk.FloorPlan.FloorNumber}",
                    PerformedAt = DateTime.UtcNow,
                    PerformedBy = "System" // TODO: Get from authentication
                };
            _context.AssetAuditLogs.Add(auditLog);

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UnassignEquipmentFromDesk(int equipmentId)
        {
            var equipment = await _context.Equipment.FindAsync(equipmentId);
            if (equipment == null)
            {
                return NotFound();
            }

            equipment.CurrentDeskId = null;
            equipment.CurrentFloorPlanId = null;
            equipment.CurrentLocationId = null;
            equipment.UpdatedAt = DateTime.UtcNow;

                            // Log the unassignment
                var auditLog = new AssetAuditLog
                {
                    EquipmentId = equipmentId,
                    Action = "Unassigned from Desk",
                    Details = "Removed from desk assignment",
                    PerformedAt = DateTime.UtcNow,
                    PerformedBy = "System" // TODO: Get from authentication
                };
            _context.AssetAuditLogs.Add(auditLog);

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetUnassignedEquipment()
        {
            var equipment = await _context.Equipment
                .Include(e => e.AssetCategory)
                .Include(e => e.CurrentStatus)
                .Include(e => e.AssignedEntraUser)
                .Where(e => e.IsActive && e.CurrentDeskId == null)
                .Select(e => new
                {
                    id = e.Id,
                    oathTag = e.OATH_Tag,
                    assetTag = e.Asset_Tag,
                    model = e.Model,
                    manufacturer = e.Manufacturer,
                    category = e.AssetCategory.Name,
                    status = e.CurrentStatus.Name,
                    statusColor = e.CurrentStatus.Color,
                    assignedUser = e.AssignedEntraUser.DisplayName,
                    assignedEmail = e.AssignedEntraUser.UserPrincipalName
                })
                .ToListAsync();

            return Json(equipment);
        }
    }
}
