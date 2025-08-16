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
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? pageNumber, int? pageSize, 
            string? categoryFilter, string? statusFilter, string? departmentFilter, 
            string? oathTagFilter, string? assignedToFilter, string? locationFilter, string? netNameFilter, string? modelFilter)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["OATHTagSortParm"] = String.IsNullOrEmpty(sortOrder) ? "oath_tag_desc" : "";
            ViewData["NetNameSortParm"] = sortOrder == "netname" ? "netname_desc" : "netname";
            ViewData["ModelSortParm"] = sortOrder == "model" ? "model_desc" : "model";
            ViewData["StatusSortParm"] = sortOrder == "status" ? "status_desc" : "status";
            ViewData["LocationSortParm"] = sortOrder == "location" ? "location_desc" : "location";
            ViewData["AssignedToSortParm"] = sortOrder == "assigned" ? "assigned_desc" : "assigned";
            ViewData["CategorySortParm"] = sortOrder == "category" ? "category_desc" : "category";
            ViewData["DepartmentSortParm"] = sortOrder == "department" ? "department_desc" : "department";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CategoryFilter"] = categoryFilter;
            ViewData["StatusFilter"] = statusFilter;
            ViewData["DepartmentFilter"] = departmentFilter;
            ViewData["OathTagFilter"] = oathTagFilter;
            ViewData["AssignedToFilter"] = assignedToFilter;
            ViewData["LocationFilter"] = locationFilter;
            ViewData["NetNameFilter"] = netNameFilter;
            ViewData["ModelFilter"] = modelFilter;
            
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
                           .Include(e => e.TechnologyConfiguration)
                           select e;

            // Apply search filter
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
                                                e.Facility.Contains(searchString) ||
                                                (e.TechnologyConfiguration != null && e.TechnologyConfiguration.NetName.Contains(searchString)));
            }

            // Apply category filter
            if (!String.IsNullOrEmpty(categoryFilter))
            {
                equipment = equipment.Where(e => e.AssetCategory != null && e.AssetCategory.Name == categoryFilter);
            }

            // Apply status filter
            if (!String.IsNullOrEmpty(statusFilter))
            {
                equipment = equipment.Where(e => e.CurrentStatus != null && e.CurrentStatus.Name == statusFilter);
            }

            // Apply department filter
            if (!String.IsNullOrEmpty(departmentFilter))
            {
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Department) && e.Department == departmentFilter);
            }

            // Apply OATH Tag filter
            if (!String.IsNullOrEmpty(oathTagFilter))
            {
                equipment = equipment.Where(e => e.OATH_Tag == oathTagFilter);
            }

            // Apply Assigned To filter
            if (!String.IsNullOrEmpty(assignedToFilter))
            {
                equipment = equipment.Where(e => e.Assigned_User_Name == assignedToFilter);
            }

            // Apply Location filter
            if (!String.IsNullOrEmpty(locationFilter))
            {
                equipment = equipment.Where(e => e.CurrentLocation != null && e.CurrentLocation.Name == locationFilter);
            }

            // Apply Net Name filter
            if (!String.IsNullOrEmpty(netNameFilter))
            {
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.NetName) && 
                                                e.TechnologyConfiguration.NetName == netNameFilter);
            }

            // Apply Model filter
            if (!String.IsNullOrEmpty(modelFilter))
            {
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Model) && e.Model == modelFilter);
            }



            equipment = sortOrder switch
            {
                "oath_tag_desc" => equipment.OrderByDescending(e => e.OATH_Tag),
                "netname" => equipment.OrderBy(e => e.TechnologyConfiguration != null ? e.TechnologyConfiguration.NetName : ""),
                "netname_desc" => equipment.OrderByDescending(e => e.TechnologyConfiguration != null ? e.TechnologyConfiguration.NetName : ""),
                "model" => equipment.OrderBy(e => e.Model),
                "model_desc" => equipment.OrderByDescending(e => e.Model),
                "status" => equipment.OrderBy(e => e.CurrentStatus.Name),
                "status_desc" => equipment.OrderByDescending(e => e.CurrentStatus.Name),
                "location" => equipment.OrderBy(e => e.CurrentLocation.Name),
                "location_desc" => equipment.OrderByDescending(e => e.CurrentLocation.Name),
                "assigned" => equipment.OrderBy(e => e.Assigned_User_Name),
                "assigned_desc" => equipment.OrderByDescending(e => e.Assigned_User_Name),
                "category" => equipment.OrderBy(e => e.AssetCategory.Name),
                "category_desc" => equipment.OrderByDescending(e => e.AssetCategory.Name),
                "department" => equipment.OrderBy(e => e.Department),
                "department_desc" => equipment.OrderByDescending(e => e.Department),
                _ => equipment.OrderBy(e => e.OATH_Tag),
            };

            // Get filter options for dropdowns
            ViewData["CategoryOptions"] = await _context.AssetCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => c.Name)
                .ToListAsync();

            ViewData["StatusOptions"] = await _context.AssetStatuses
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .Select(s => s.Name)
                .ToListAsync();

            ViewData["DepartmentOptions"] = await _context.Equipment
                .Where(e => !String.IsNullOrEmpty(e.Department))
                .Select(e => e.Department)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            // Get OATH Tag options
            ViewData["OathTagOptions"] = await _context.Equipment
                .Where(e => !String.IsNullOrEmpty(e.OATH_Tag))
                .Select(e => e.OATH_Tag)
                .Distinct()
                .OrderBy(o => o)
                .ToListAsync();

            // Get Assigned To options
            ViewData["AssignedToOptions"] = await _context.Equipment
                .Where(e => !String.IsNullOrEmpty(e.Assigned_User_Name))
                .Select(e => e.Assigned_User_Name)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            // Get Location options
            ViewData["LocationOptions"] = await _context.Equipment
                .Where(e => e.CurrentLocation != null)
                .Select(e => e.CurrentLocation.Name)
                .Distinct()
                .OrderBy(l => l)
                .ToListAsync();

            // Get Net Name options
            ViewData["NetNameOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.NetName))
                .Select(tc => tc.NetName)
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();

            // Get Model options
            ViewData["ModelOptions"] = await _context.Equipment
                .Where(e => !String.IsNullOrEmpty(e.Model))
                .Select(e => e.Model)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();

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

            var equipment = await _context.Equipment
                .Include(e => e.AssetCategory)
                .Include(e => e.CurrentStatus)
                .Include(e => e.CurrentLocation)
                .Include(e => e.CurrentFloorPlan)
                .Include(e => e.CurrentDesk)
                .Include(e => e.AssignedPerson)
                .Include(e => e.AssignedEntraUser)
                .Include(e => e.TechnologyConfiguration)
                .FirstOrDefaultAsync(e => e.Id == id);
            
            if (equipment == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await GetAssetCategorySelectList();
            ViewBag.Statuses = await GetAssetStatusSelectList();
            
            // Create location SelectList with the current equipment's location pre-selected
            var locations = await _context.Locations
                .Where(l => l.IsActive)
                .OrderBy(l => l.Name)
                .ToListAsync();
            ViewBag.Locations = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(locations, "Id", "Name", equipment.CurrentLocationId);
            
            // FloorPlans and Desks will be loaded dynamically via JavaScript
            ViewBag.FloorPlans = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(new List<object>(), "Id", "DisplayText");
            ViewBag.Desks = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(new List<object>(), "Id", "DisplayText");
            ViewBag.Persons = await GetPersonSelectList();
            ViewBag.EntraUsers = await GetEntraUserSelectList();

            return View(equipment);
        }

        // POST: Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Equipment equipment)
        {
            if (id != equipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing equipment with all related data
                    var existingEquipment = await _context.Equipment
                        .Include(e => e.TechnologyConfiguration)
                        .Include(e => e.CurrentStatus)
                        .Include(e => e.CurrentLocation)
                        .Include(e => e.AssignedPerson)
                        .FirstOrDefaultAsync(e => e.Id == id);

                    if (existingEquipment == null)
                    {
                        return NotFound();
                    }

                    // Store original for audit logging
                    var originalEquipment = await _context.Equipment
                        .Include(e => e.CurrentStatus)
                        .Include(e => e.CurrentLocation)
                        .Include(e => e.AssignedPerson)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.Id == id);

                    // Update Equipment properties
                    existingEquipment.Serial_Number = equipment.Serial_Number;
                    existingEquipment.Manufacturer = equipment.Manufacturer;
                    existingEquipment.Model = equipment.Model;
                    existingEquipment.Computer_Name = equipment.Computer_Name;
                    existingEquipment.IP_Address = equipment.IP_Address;
                    existingEquipment.Phone_Number = equipment.Phone_Number;
                    existingEquipment.OS_Version = equipment.OS_Version;
                    existingEquipment.Service_Tag = equipment.Service_Tag;
                    existingEquipment.AssetCategoryId = equipment.AssetCategoryId;
                    existingEquipment.CurrentStatusId = equipment.CurrentStatusId;
                    existingEquipment.Condition = equipment.Condition;
                    existingEquipment.Purchase_Date = equipment.Purchase_Date;
                    existingEquipment.Purchase_Cost = equipment.Purchase_Cost;
                    existingEquipment.Warranty_Expiry = equipment.Warranty_Expiry;
                    existingEquipment.CurrentLocationId = equipment.CurrentLocationId;
                    existingEquipment.CurrentFloorPlanId = equipment.CurrentFloorPlanId;
                    existingEquipment.CurrentDeskId = equipment.CurrentDeskId;
                    existingEquipment.AssignedPersonId = equipment.AssignedPersonId;
                    existingEquipment.AssignedEntraUserId = equipment.AssignedEntraUserId;
                    existingEquipment.Notes = equipment.Notes;
                    existingEquipment.UpdatedAt = DateTime.UtcNow;
                    existingEquipment.UpdatedBy = User.Identity?.Name ?? "System";

                    // Handle TechnologyConfiguration
                    if (equipment.TechnologyConfiguration != null)
                    {
                        if (existingEquipment.TechnologyConfiguration == null)
                        {
                            // Create new TechnologyConfiguration
                            existingEquipment.TechnologyConfiguration = new TechnologyConfiguration
                            {
                                EquipmentId = existingEquipment.Id,
                                MACAddress = equipment.TechnologyConfiguration.MACAddress,
                                WallPort = equipment.TechnologyConfiguration.WallPort,
                                SwitchName = equipment.TechnologyConfiguration.SwitchName,
                                SwitchPort = equipment.TechnologyConfiguration.SwitchPort,
                                Extension = equipment.TechnologyConfiguration.Extension,
                                IMEI = equipment.TechnologyConfiguration.IMEI,
                                SIMCardNumber = equipment.TechnologyConfiguration.SIMCardNumber,
                                ConfigurationNotes = equipment.TechnologyConfiguration.ConfigurationNotes,
                                LastUpdated = DateTime.UtcNow
                            };
                        }
                        else
                        {
                            // Update existing TechnologyConfiguration
                            existingEquipment.TechnologyConfiguration.MACAddress = equipment.TechnologyConfiguration.MACAddress;
                            existingEquipment.TechnologyConfiguration.WallPort = equipment.TechnologyConfiguration.WallPort;
                            existingEquipment.TechnologyConfiguration.SwitchName = equipment.TechnologyConfiguration.SwitchName;
                            existingEquipment.TechnologyConfiguration.SwitchPort = equipment.TechnologyConfiguration.SwitchPort;
                            existingEquipment.TechnologyConfiguration.Extension = equipment.TechnologyConfiguration.Extension;
                            existingEquipment.TechnologyConfiguration.IMEI = equipment.TechnologyConfiguration.IMEI;
                            existingEquipment.TechnologyConfiguration.SIMCardNumber = equipment.TechnologyConfiguration.SIMCardNumber;
                            existingEquipment.TechnologyConfiguration.ConfigurationNotes = equipment.TechnologyConfiguration.ConfigurationNotes;
                            existingEquipment.TechnologyConfiguration.LastUpdated = DateTime.UtcNow;
                        }
                    }

                    await _context.SaveChangesAsync();

                    // Log the changes
                    await LogEquipmentChanges(originalEquipment, existingEquipment);
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

        // POST: Equipment/UpdateField/5
        [HttpPost]
        public async Task<IActionResult> UpdateField(int id, [FromBody] UpdateFieldRequest request)
        {
            try
            {
                var equipment = await _context.Equipment.FindAsync(id);
                if (equipment == null)
                {
                    return Json(new { success = false, message = "Equipment not found" });
                }

                // Use reflection to update the field
                var property = typeof(Equipment).GetProperty(request.FieldName);
                if (property == null)
                {
                    return Json(new { success = false, message = "Field not found" });
                }

                // Check if the property is writable
                if (!property.CanWrite)
                {
                    return Json(new { success = false, message = "Field is read-only" });
                }

                // Convert and set the value
                object? convertedValue = null;
                if (!string.IsNullOrEmpty(request.FieldValue))
                {
                    if (property.PropertyType == typeof(string))
                    {
                        convertedValue = request.FieldValue;
                    }
                    else if (property.PropertyType == typeof(int?) || property.PropertyType == typeof(int))
                    {
                        if (int.TryParse(request.FieldValue, out int intValue))
                            convertedValue = intValue;
                    }
                    else if (property.PropertyType == typeof(decimal?) || property.PropertyType == typeof(decimal))
                    {
                        if (decimal.TryParse(request.FieldValue, out decimal decimalValue))
                            convertedValue = decimalValue;
                    }
                    else if (property.PropertyType == typeof(DateTime?) || property.PropertyType == typeof(DateTime))
                    {
                        if (DateTime.TryParse(request.FieldValue, out DateTime dateValue))
                            convertedValue = dateValue;
                    }
                    else
                    {
                        convertedValue = request.FieldValue;
                    }
                }

                // Set the value
                property.SetValue(equipment, convertedValue);
                
                // Update audit fields
                equipment.UpdatedAt = DateTime.UtcNow;
                equipment.UpdatedBy = User.Identity?.Name ?? "System";

                // Save changes
                await _context.SaveChangesAsync();

                // Log the change
                await LogEquipmentAction(id, "Field Updated", $"Updated {request.FieldName} to: {request.FieldValue ?? "(empty)"}");

                return Json(new { success = true, message = "Field updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
                .OrderBy(fp => fp.Location!.Name)
                .ThenBy(fp => fp.FloorNumber)
                .Select(fp => new { 
                    Id = fp.Id, 
                    DisplayText = $"{fp.Location!.Name} - Floor {fp.FloorNumber}"
                })
                .ToListAsync();
            return new Microsoft.AspNetCore.Mvc.Rendering.SelectList(floorPlans, "Id", "DisplayText");
        }

        private async Task<Microsoft.AspNetCore.Mvc.Rendering.SelectList> GetDeskSelectList()
        {
            var desks = await _context.Desks
                .Where(d => d.IsActive)
                .OrderBy(d => d.DeskNumber)
                .Select(d => new {
                    Id = d.Id,
                    DisplayText = $"{d.DeskNumber}{(d.DeskName != null ? " - " + d.DeskName : "")}"
                })
                .ToListAsync();
            return new Microsoft.AspNetCore.Mvc.Rendering.SelectList(desks, "Id", "DisplayText");
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

    public class UpdateFieldRequest
    {
        public string FieldName { get; set; } = string.Empty;
        public string? FieldValue { get; set; }
    }
}
