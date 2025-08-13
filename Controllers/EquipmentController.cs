using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;

namespace AssetManagement.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EquipmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Equipment
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? pageNumber, int? pageSize)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["OATHTagSortParm"] = String.IsNullOrEmpty(sortOrder) ? "oath_tag_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["LocationSortParm"] = sortOrder == "Location" ? "location_desc" : "Location";
            ViewData["CurrentFilter"] = searchString;
            
            // Page size options
            var pageSizeOptions = new List<int> { 10, 25, 50, 100 };
            ViewData["PageSizeOptions"] = pageSizeOptions;
            ViewData["CurrentPageSize"] = pageSize ?? 20;

            var equipment = from e in _context.Equipment
                           .Include(e => e.AssetCategory)
                           .Include(e => e.CurrentStatus)
                           .Include(e => e.CurrentLocation)
                           .Include(e => e.CurrentFloorPlan)
                           .Include(e => e.CurrentDesk)
                           .Include(e => e.AssignedPerson)
                           .Include(e => e.AssignedEntraUser)
                           select e;

            if (!String.IsNullOrEmpty(searchString))
            {
                equipment = equipment.Where(e => e.OATH_Tag.Contains(searchString) ||
                                                e.Serial_Number.Contains(searchString) ||
                                                e.Computer_Name.Contains(searchString) ||
                                                e.Assigned_User_Name.Contains(searchString) ||
                                                e.Assigned_User_Email.Contains(searchString) ||
                                                e.Model.Contains(searchString) ||
                                                e.Manufacturer.Contains(searchString) ||
                                                e.Department.Contains(searchString) ||
                                                e.Facility.Contains(searchString));
            }

            equipment = sortOrder switch
            {
                "oath_tag_desc" => equipment.OrderByDescending(e => e.OATH_Tag),
                "Name" => equipment.OrderBy(e => e.Computer_Name),
                "name_desc" => equipment.OrderByDescending(e => e.Computer_Name),
                "Status" => equipment.OrderBy(e => e.CurrentStatus.Name),
                "status_desc" => equipment.OrderByDescending(e => e.CurrentStatus.Name),
                "Location" => equipment.OrderBy(e => e.CurrentLocation.Name),
                "location_desc" => equipment.OrderByDescending(e => e.CurrentLocation.Name),
                _ => equipment.OrderBy(e => e.OATH_Tag),
            };

            int currentPageSize = pageSize ?? 20;
            return View(await PaginatedList<Equipment>.CreateAsync(equipment.AsNoTracking(), pageNumber ?? 1, currentPageSize));
        }

        // GET: Equipment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .Include(e => e.AssetCategory)
                .Include(e => e.CurrentStatus)
                .Include(e => e.CurrentLocation)
                .Include(e => e.CurrentFloorPlan)
                .Include(e => e.CurrentDesk)
                .Include(e => e.AssignedPerson)
                .Include(e => e.AssignedEntraUser)
                .Include(e => e.AuditLogs.OrderByDescending(a => a.PerformedAt).Take(10))
                .FirstOrDefaultAsync(m => m.Id == id);

            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // GET: Equipment/GetSearchSuggestions
        [HttpGet]
        public async Task<IActionResult> GetSearchSuggestions(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Json(new List<object>());
            }

            var suggestions = new List<object>();

            // Get OATH Tag suggestions
            var oathTags = await _context.Equipment
                .Where(e => e.OATH_Tag.Contains(query))
                .Select(e => new { value = e.OATH_Tag, type = "OATH Tag", description = e.Model })
                .Take(5)
                .ToListAsync();
            suggestions.AddRange(oathTags);

            // Get Serial Number suggestions
            var serialNumbers = await _context.Equipment
                .Where(e => !string.IsNullOrEmpty(e.Serial_Number) && e.Serial_Number.Contains(query))
                .Select(e => new { value = e.Serial_Number, type = "Serial Number", description = e.OATH_Tag })
                .Take(5)
                .ToListAsync();
            suggestions.AddRange(serialNumbers);

            // Get Net Name suggestions
            var netNames = await _context.Equipment
                .Where(e => !string.IsNullOrEmpty(e.Computer_Name) && e.Computer_Name.Contains(query))
                .Select(e => new { value = e.Computer_Name, type = "Net Name", description = e.OATH_Tag })
                .Take(5)
                .ToListAsync();
            suggestions.AddRange(netNames);

            // Get Model suggestions
            var models = await _context.Equipment
                .Where(e => !string.IsNullOrEmpty(e.Model) && e.Model.Contains(query))
                .Select(e => e.Model)
                .Distinct()
                .Take(5)
                .Select(m => new { value = m, type = "Model", description = (string?)null })
                .ToListAsync();
            suggestions.AddRange(models);

            // Get Manufacturer suggestions
            var manufacturers = await _context.Equipment
                .Where(e => !string.IsNullOrEmpty(e.Manufacturer) && e.Manufacturer.Contains(query))
                .Select(e => e.Manufacturer)
                .Distinct()
                .Take(5)
                .Select(m => new { value = m, type = "Manufacturer", description = (string?)null })
                .ToListAsync();
            suggestions.AddRange(manufacturers);

            // Get User suggestions
            var users = await _context.Equipment
                .Where(e => !string.IsNullOrEmpty(e.Assigned_User_Name) && e.Assigned_User_Name.Contains(query))
                .Select(e => new { value = e.Assigned_User_Name, type = "User", description = e.Assigned_User_Email })
                .Take(5)
                .ToListAsync();
            suggestions.AddRange(users);

            return Json(suggestions.Take(15).ToList());
        }

        // GET: Equipment/Create
        public async Task<IActionResult> Create()
        {
            ViewData["AssetCategoryId"] = await GetAssetCategorySelectList();
            ViewData["CurrentStatusId"] = await GetAssetStatusSelectList();
            ViewData["CurrentLocationId"] = await GetLocationSelectList();
            ViewData["CurrentFloorPlanId"] = await GetFloorPlanSelectList();
            ViewData["CurrentDeskId"] = await GetDeskSelectList();
            ViewData["AssignedPersonId"] = await GetPersonSelectList();
            ViewData["AssignedEntraUserId"] = await GetEntraUserSelectList();

            return View();
        }

        // POST: Equipment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OATH_Tag,Serial_Number,AssetCategoryId,Manufacturer,Model,Computer_Name,PurchaseOrderNumber,PurchasePrice,CostCentre,PurchaseDate,WarrantyStartDate,WarrantyEndDate,Condition,Barcode,QRCode,Assigned_User_Name,Assigned_User_Email,Assigned_User_ID,AssignedPersonId,AssignedEntraUserId,AssignedEntraObjectId,Phone_Number,ExpectedReturnDate,CurrentStatusId,Department,Facility,OS_Version,IP_Address,CurrentLocationId,CurrentFloorPlanId,CurrentDeskId,Current_Location_Notes,CurrentBookValue,DepreciationRate,LastMaintenanceDate,NextMaintenanceDate")] Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                equipment.CreatedAt = DateTime.UtcNow;
                equipment.CreatedBy = User.Identity?.Name ?? "System";
                
                _context.Add(equipment);
                await _context.SaveChangesAsync();

                // Log the creation
                await LogEquipmentAction(equipment.Id, "Created", "Equipment created");

                return RedirectToAction(nameof(Index));
            }

            ViewData["AssetCategoryId"] = await GetAssetCategorySelectList();
            ViewData["CurrentStatusId"] = await GetAssetStatusSelectList();
            ViewData["CurrentLocationId"] = await GetLocationSelectList();
            ViewData["CurrentFloorPlanId"] = await GetFloorPlanSelectList();
            ViewData["CurrentDeskId"] = await GetDeskSelectList();
            ViewData["AssignedPersonId"] = await GetPersonSelectList();
            ViewData["AssignedEntraUserId"] = await GetEntraUserSelectList();

            return View(equipment);
        }

        // GET: Equipment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            ViewData["AssetCategoryId"] = await GetAssetCategorySelectList();
            ViewData["CurrentStatusId"] = await GetAssetStatusSelectList();
            ViewData["CurrentLocationId"] = await GetLocationSelectList();
            ViewData["CurrentFloorPlanId"] = await GetFloorPlanSelectList();
            ViewData["CurrentDeskId"] = await GetDeskSelectList();
            ViewData["AssignedPersonId"] = await GetPersonSelectList();
            ViewData["AssignedEntraUserId"] = await GetEntraUserSelectList();

            return View(equipment);
        }

        // POST: Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OATH_Tag,Serial_Number,AssetCategoryId,Manufacturer,Model,Computer_Name,PurchaseOrderNumber,PurchasePrice,CostCentre,PurchaseDate,WarrantyStartDate,WarrantyEndDate,Condition,Barcode,QRCode,Assigned_User_Name,Assigned_User_Email,Assigned_User_ID,AssignedPersonId,AssignedEntraUserId,AssignedEntraObjectId,Phone_Number,ExpectedReturnDate,CurrentStatusId,Department,Facility,OS_Version,IP_Address,CurrentLocationId,CurrentFloorPlanId,CurrentDeskId,Current_Location_Notes,CurrentBookValue,DepreciationRate,LastMaintenanceDate,NextMaintenanceDate,CreatedAt")] Equipment equipment)
        {
            if (id != equipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var originalEquipment = await _context.Equipment
                        .Include(e => e.CurrentStatus)
                        .Include(e => e.CurrentLocation)
                        .Include(e => e.AssignedPerson)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.Id == id);

                    equipment.UpdatedAt = DateTime.UtcNow;
                    equipment.UpdatedBy = User.Identity?.Name ?? "System";

                    _context.Update(equipment);
                    await _context.SaveChangesAsync();

                    // Log the changes
                    await LogEquipmentChanges(originalEquipment, equipment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipmentExists(equipment.Id))
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

            ViewData["AssetCategoryId"] = await GetAssetCategorySelectList();
            ViewData["CurrentStatusId"] = await GetAssetStatusSelectList();
            ViewData["CurrentLocationId"] = await GetLocationSelectList();
            ViewData["CurrentFloorPlanId"] = await GetFloorPlanSelectList();
            ViewData["CurrentDeskId"] = await GetDeskSelectList();
            ViewData["AssignedPersonId"] = await GetPersonSelectList();
            ViewData["AssignedEntraUserId"] = await GetEntraUserSelectList();

            return View(equipment);
        }

        // GET: Equipment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .Include(e => e.AssetCategory)
                .Include(e => e.CurrentStatus)
                .Include(e => e.CurrentLocation)
                .Include(e => e.CurrentFloorPlan)
                .Include(e => e.CurrentDesk)
                .Include(e => e.AssignedPerson)
                .Include(e => e.AssignedEntraUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // POST: Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment != null)
            {
                // Log the deletion BEFORE deleting the equipment
                await LogEquipmentAction(id, "Deleted", "Equipment deleted");
                
                _context.Equipment.Remove(equipment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Equipment/CheckOut/5
        public async Task<IActionResult> CheckOut(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .Include(e => e.AssetCategory)
                .Include(e => e.CurrentStatus)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // POST: Equipment/CheckOut/5
        [HttpPost, ActionName("CheckOut")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOutConfirmed(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment != null)
            {
                equipment.IsCheckedOut = true;
                equipment.CheckedOutDate = DateTime.UtcNow;
                equipment.CheckedOutByUserId = User.Identity?.Name ?? "System";
                equipment.UpdatedAt = DateTime.UtcNow;
                equipment.UpdatedBy = User.Identity?.Name ?? "System";

                _context.Update(equipment);
                await _context.SaveChangesAsync();

                // Log the check out
                await LogEquipmentAction(id, "Checked Out", "Equipment checked out");
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Equipment/CheckIn/5
        public async Task<IActionResult> CheckIn(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .Include(e => e.AssetCategory)
                .Include(e => e.CurrentStatus)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // POST: Equipment/CheckIn/5
        [HttpPost, ActionName("CheckIn")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckInConfirmed(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment != null)
            {
                equipment.IsCheckedOut = false;
                equipment.LastCheckInDate = DateTime.UtcNow;
                equipment.CheckedOutDate = null;
                equipment.CheckedOutByUserId = null;
                equipment.UpdatedAt = DateTime.UtcNow;
                equipment.UpdatedBy = User.Identity?.Name ?? "System";

                _context.Update(equipment);
                await _context.SaveChangesAsync();

                // Log the check in
                await LogEquipmentAction(id, "Checked In", "Equipment checked in");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EquipmentExists(int id)
        {
            return _context.Equipment.Any(e => e.Id == id);
        }

        private async Task<Microsoft.AspNetCore.Mvc.Rendering.SelectList> GetAssetCategorySelectList()
        {
            var categories = await _context.AssetCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return new Microsoft.AspNetCore.Mvc.Rendering.SelectList(categories, "Id", "Name");
        }

        private async Task<Microsoft.AspNetCore.Mvc.Rendering.SelectList> GetAssetStatusSelectList()
        {
            var statuses = await _context.AssetStatuses
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
            return new Microsoft.AspNetCore.Mvc.Rendering.SelectList(statuses, "Id", "Name");
        }

        private async Task<Microsoft.AspNetCore.Mvc.Rendering.SelectList> GetLocationSelectList()
        {
            var locations = await _context.Locations
                .Where(l => l.IsActive)
                .OrderBy(l => l.Name)
                .ToListAsync();
            return new Microsoft.AspNetCore.Mvc.Rendering.SelectList(locations, "Id", "Name");
        }

        private async Task<Microsoft.AspNetCore.Mvc.Rendering.SelectList> GetFloorPlanSelectList()
        {
            var floorPlans = await _context.FloorPlans
                .Include(fp => fp.Location)
                .Where(fp => fp.IsActive)
                .OrderBy(fp => fp.Location.Name)
                .ThenBy(fp => fp.FloorNumber)
                .ToListAsync();
            return new Microsoft.AspNetCore.Mvc.Rendering.SelectList(floorPlans, "Id", "FloorNumber", null, "Location.Name");
        }

        private async Task<Microsoft.AspNetCore.Mvc.Rendering.SelectList> GetDeskSelectList()
        {
            var desks = await _context.Desks
                .Include(d => d.FloorPlan)
                .ThenInclude(fp => fp.Location)
                .Where(d => d.IsActive)
                .OrderBy(d => d.FloorPlan.Location.Name)
                .ThenBy(d => d.FloorPlan.FloorNumber)
                .ThenBy(d => d.DeskNumber)
                .ToListAsync();
            return new Microsoft.AspNetCore.Mvc.Rendering.SelectList(desks, "Id", "DeskNumber", null, "FloorPlan.Location.Name - FloorPlan.FloorNumber");
        }

        private async Task<Microsoft.AspNetCore.Mvc.Rendering.SelectList> GetPersonSelectList()
        {
            var people = await _context.People
                .Where(p => p.IsActive)
                .OrderBy(p => p.FullName)
                .ToListAsync();
            return new Microsoft.AspNetCore.Mvc.Rendering.SelectList(people, "Id", "FullName");
        }

        private async Task<Microsoft.AspNetCore.Mvc.Rendering.SelectList> GetEntraUserSelectList()
        {
            var entraUsers = await _context.EntraUsers
                .Where(eu => eu.AccountEnabled)
                .OrderBy(eu => eu.DisplayName)
                .ToListAsync();
            return new Microsoft.AspNetCore.Mvc.Rendering.SelectList(entraUsers, "Id", "DisplayName");
        }

        private async Task LogEquipmentAction(int equipmentId, string action, string description)
        {
            var auditLog = new AssetAuditLog
            {
                EquipmentId = equipmentId,
                Action = action,
                PerformedBy = User.Identity?.Name ?? "System",
                PerformedByEmail = User.Identity?.Name ?? "system@oath.nyc.gov",
                PerformedAt = DateTime.UtcNow,
                AdditionalData = description,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            };

            _context.AssetAuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        private async Task LogEquipmentChanges(Equipment? original, Equipment updated)
        {
            if (original == null) return;

            var changes = new List<string>();

            if (original.CurrentStatusId != updated.CurrentStatusId)
            {
                var oldStatus = await _context.AssetStatuses.FindAsync(original.CurrentStatusId);
                var newStatus = await _context.AssetStatuses.FindAsync(updated.CurrentStatusId);
                changes.Add($"Status changed from '{oldStatus?.Name ?? "None"}' to '{newStatus?.Name ?? "None"}'");
            }

            if (original.CurrentLocationId != updated.CurrentLocationId)
            {
                var oldLocation = await _context.Locations.FindAsync(original.CurrentLocationId);
                var newLocation = await _context.Locations.FindAsync(updated.CurrentLocationId);
                changes.Add($"Location changed from '{oldLocation?.Name ?? "None"}' to '{newLocation?.Name ?? "None"}'");
            }

            if (original.AssignedPersonId != updated.AssignedPersonId)
            {
                var oldPerson = await _context.People.FindAsync(original.AssignedPersonId);
                var newPerson = await _context.People.FindAsync(updated.AssignedPersonId);
                changes.Add($"Assigned person changed from '{oldPerson?.FullName ?? "None"}' to '{newPerson?.FullName ?? "None"}'");
            }

            if (changes.Any())
            {
                var auditLog = new AssetAuditLog
                {
                    EquipmentId = updated.Id,
                    Action = "Updated",
                    PerformedBy = User.Identity?.Name ?? "System",
                    PerformedByEmail = User.Identity?.Name ?? "system@oath.nyc.gov",
                    PerformedAt = DateTime.UtcNow,
                    AdditionalData = string.Join("; ", changes),
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
                };

                _context.AssetAuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
