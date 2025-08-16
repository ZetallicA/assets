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
            string? oathTagFilter, string? assignedToFilter, string? locationFilter, string? floorFilter, string? netNameFilter, string? modelFilter, string? serialNumberFilter,
            string? deskNumberFilter, string? manufacturerFilter, string? purchaseDateFilter)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["OATHTagSortParm"] = String.IsNullOrEmpty(sortOrder) ? "oath_tag_desc" : "";
            ViewData["NetNameSortParm"] = sortOrder == "netname" ? "netname_desc" : "netname";
            ViewData["ModelSortParm"] = sortOrder == "model" ? "model_desc" : "model";
            ViewData["StatusSortParm"] = sortOrder == "status" ? "status_desc" : "status";
            ViewData["LocationSortParm"] = sortOrder == "location" ? "location_desc" : "location";
            ViewData["FloorSortParm"] = sortOrder == "floor" ? "floor_desc" : "floor";
            ViewData["AssignedToSortParm"] = sortOrder == "assigned" ? "assigned_desc" : "assigned";
            ViewData["CategorySortParm"] = sortOrder == "category" ? "category_desc" : "category";
            ViewData["DepartmentSortParm"] = sortOrder == "department" ? "department_desc" : "department";
            ViewData["SerialNumberSortParm"] = sortOrder == "serialnumber" ? "serialnumber_desc" : "serialnumber";
            ViewData["DeskNumberSortParm"] = sortOrder == "desknumber" ? "desknumber_desc" : "desknumber";
            ViewData["ManufacturerSortParm"] = sortOrder == "manufacturer" ? "manufacturer_desc" : "manufacturer";
            ViewData["PurchaseDateSortParm"] = sortOrder == "purchasedate" ? "purchasedate_desc" : "purchasedate";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CategoryFilter"] = categoryFilter;
            ViewData["StatusFilter"] = statusFilter;
            ViewData["DepartmentFilter"] = departmentFilter;
            ViewData["OathTagFilter"] = oathTagFilter;
            ViewData["AssignedToFilter"] = assignedToFilter;
            ViewData["LocationFilter"] = locationFilter;
            ViewData["FloorFilter"] = floorFilter;
            ViewData["NetNameFilter"] = netNameFilter;
            ViewData["ModelFilter"] = modelFilter;
            ViewData["SerialNumberFilter"] = serialNumberFilter;
            ViewData["DeskNumberFilter"] = deskNumberFilter;
            ViewData["ManufacturerFilter"] = manufacturerFilter;
            ViewData["PurchaseDateFilter"] = purchaseDateFilter;
            
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
                                                (e.CurrentLocation != null && e.CurrentLocation.Name.Contains(searchString)) ||
                                                (e.AssetCategory != null && e.AssetCategory.Name.Contains(searchString)) ||
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

            // Apply Location filter (Building only)
            if (!String.IsNullOrEmpty(locationFilter))
            {
                equipment = equipment.Where(e => e.CurrentLocation != null && e.CurrentLocation.Name.StartsWith(locationFilter));
            }

            // Apply Floor filter
            if (!String.IsNullOrEmpty(floorFilter))
            {
                equipment = equipment.Where(e => e.CurrentFloorPlan != null && e.CurrentFloorPlan.FloorName == floorFilter);
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

            // Apply Serial Number filter
            if (!String.IsNullOrEmpty(serialNumberFilter))
            {
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Serial_Number) && e.Serial_Number == serialNumberFilter);
            }

            // Apply Desk Number filter
            if (!String.IsNullOrEmpty(deskNumberFilter))
            {
                equipment = equipment.Where(e => e.CurrentDesk != null && e.CurrentDesk.DeskName == deskNumberFilter);
            }

            // Apply Manufacturer filter
            if (!String.IsNullOrEmpty(manufacturerFilter))
            {
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Manufacturer) && e.Manufacturer == manufacturerFilter);
            }

            // Apply Purchase Date filter
            if (!String.IsNullOrEmpty(purchaseDateFilter))
            {
                equipment = equipment.Where(e => e.PurchaseDate.HasValue && e.PurchaseDate.Value.ToString("MM/dd/yyyy") == purchaseDateFilter);
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
            "floor" => equipment.OrderBy(e => e.CurrentFloorPlan != null ? e.CurrentFloorPlan.FloorName : ""),
            "floor_desc" => equipment.OrderByDescending(e => e.CurrentFloorPlan != null ? e.CurrentFloorPlan.FloorName : ""),
            "assigned" => equipment.OrderBy(e => e.Assigned_User_Name),
                "assigned_desc" => equipment.OrderByDescending(e => e.Assigned_User_Name),
                "category" => equipment.OrderBy(e => e.AssetCategory.Name),
                "category_desc" => equipment.OrderByDescending(e => e.AssetCategory.Name),
                "department" => equipment.OrderBy(e => e.Department),
                "department_desc" => equipment.OrderByDescending(e => e.Department),
                "serialnumber" => equipment.OrderBy(e => e.Serial_Number),
                "serialnumber_desc" => equipment.OrderByDescending(e => e.Serial_Number),
                "desknumber" => equipment.OrderBy(e => e.CurrentDesk != null ? e.CurrentDesk.DeskName : ""),
                "desknumber_desc" => equipment.OrderByDescending(e => e.CurrentDesk != null ? e.CurrentDesk.DeskName : ""),
                "manufacturer" => equipment.OrderBy(e => e.Manufacturer),
                "manufacturer_desc" => equipment.OrderByDescending(e => e.Manufacturer),
                "purchasedate" => equipment.OrderBy(e => e.PurchaseDate),
                "purchasedate_desc" => equipment.OrderByDescending(e => e.PurchaseDate),
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

            // Get Location options (Building only)
            var locationNames = await _context.Equipment
                .Where(e => e.CurrentLocation != null)
                .Select(e => e.CurrentLocation.Name)
                .Distinct()
                .OrderBy(l => l)
                .ToListAsync();
            
            ViewData["LocationOptions"] = locationNames
                .Select(name => name.Split(" - ")[0])
                .Distinct()
                .OrderBy(l => l)
                .ToList();

            // Get Floor options
            ViewData["FloorOptions"] = await _context.Equipment
                .Where(e => e.CurrentFloorPlan != null)
                .Select(e => e.CurrentFloorPlan.FloorName)
                .Distinct()
                .OrderBy(f => f)
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

            // Get Serial Number options
            ViewData["SerialNumberOptions"] = await _context.Equipment
                .Where(e => !String.IsNullOrEmpty(e.Serial_Number))
                .Select(e => e.Serial_Number)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            // Get Desk Number options
            ViewData["DeskNumberOptions"] = await _context.Equipment
                .Where(e => e.CurrentDesk != null && !String.IsNullOrEmpty(e.CurrentDesk.DeskName))
                .Select(e => e.CurrentDesk.DeskName)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            // Get Manufacturer options
            ViewData["ManufacturerOptions"] = await _context.Equipment
                .Where(e => !String.IsNullOrEmpty(e.Manufacturer))
                .Select(e => e.Manufacturer)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();

            // Get Purchase Date options
            var purchaseDates = await _context.Equipment
                .Where(e => e.PurchaseDate.HasValue)
                .Select(e => e.PurchaseDate.Value)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();
            
            ViewData["PurchaseDateOptions"] = purchaseDates
                .Select(d => d.ToString("MM/dd/yyyy"))
                .ToList();

            // Get Asset Tag options
            ViewData["AssetTagOptions"] = await _context.Equipment
                .Where(e => !String.IsNullOrEmpty(e.Asset_Tag))
                .Select(e => e.Asset_Tag)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            // Get Service Tag options
            ViewData["ServiceTagOptions"] = await _context.Equipment
                .Where(e => !String.IsNullOrEmpty(e.Service_Tag))
                .Select(e => e.Service_Tag)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            // Get IP Address options
            ViewData["IPAddressOptions"] = await _context.Equipment
                .Where(e => !String.IsNullOrEmpty(e.IP_Address))
                .Select(e => e.IP_Address)
                .Distinct()
                .OrderBy(i => i)
                .ToListAsync();

            // Get OS Version options
            ViewData["OSVersionOptions"] = await _context.Equipment
                .Where(e => !String.IsNullOrEmpty(e.OS_Version))
                .Select(e => e.OS_Version)
                .Distinct()
                .OrderBy(o => o)
                .ToListAsync();

            // Get Phone Number options
            ViewData["PhoneNumberOptions"] = await _context.Equipment
                .Where(e => !String.IsNullOrEmpty(e.Phone_Number))
                .Select(e => e.Phone_Number)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();

            // Get Purchase Price options
            var purchasePrices = await _context.Equipment
                .Where(e => e.PurchasePrice.HasValue)
                .Select(e => e.PurchasePrice.Value)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();
            
            ViewData["PurchasePriceOptions"] = purchasePrices
                .Select(p => p.ToString("C"))
                .ToList();

            // Get Warranty End options
            var warrantyEndDates = await _context.Equipment
                .Where(e => e.WarrantyEndDate.HasValue)
                .Select(e => e.WarrantyEndDate.Value)
                .Distinct()
                .OrderBy(w => w)
                .ToListAsync();
            
            ViewData["WarrantyEndOptions"] = warrantyEndDates
                .Select(d => d.ToString("MM/dd/yyyy"))
                .ToList();

            // Get Notes options (first 50 characters for filtering)
            ViewData["NotesOptions"] = await _context.Equipment
                .Where(e => !String.IsNullOrEmpty(e.Notes))
                .Select(e => e.Notes.Length > 50 ? e.Notes.Substring(0, 50) + "..." : e.Notes)
                .Distinct()
                .OrderBy(n => n)
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

        // DataTable Inline Editing
        [HttpPost]
        [Route("Equipment/UpdateField")]
        public async Task<IActionResult> UpdateField([FromBody] InlineEditRequest request)
        {
            try
            {
                // Debug logging
                Console.WriteLine($"UpdateField called with ID: {request.Id}, Field: {request.Field}, Value: {request.Value}");
                
                // Try to find equipment by OATH tag first, then by ID
                var equipment = await _context.Equipment
                    .Include(e => e.TechnologyConfiguration)
                    .FirstOrDefaultAsync(e => e.OATH_Tag == request.Id || e.Id.ToString() == request.Id);
                
                Console.WriteLine($"Looking for equipment with OATH Tag or ID: {request.Id}");
                Console.WriteLine($"Equipment found: {equipment != null}");
                
                if (equipment == null)
                {
                    // Get some sample equipment for debugging
                    var sampleEquipment = await _context.Equipment.Take(10).Select(e => new { e.Id, e.OATH_Tag }).ToListAsync();
                    Console.WriteLine($"Sample equipment in database: {string.Join(", ", sampleEquipment.Select(e => $"{e.Id}({e.OATH_Tag})"))}");
                    
                    return Json(new InlineEditResponse { 
                        Success = false, 
                        Message = $"Equipment with OATH Tag/ID {request.Id} not found. Sample equipment: {string.Join(", ", sampleEquipment.Select(e => $"{e.OATH_Tag}(ID:{e.Id})"))}. Please refresh the page and try again." 
                    });
                }

                var originalEquipment = new Equipment
                {
                    Id = equipment.Id,
                    CurrentStatusId = equipment.CurrentStatusId,
                    CurrentLocationId = equipment.CurrentLocationId,
                    AssignedPersonId = equipment.AssignedPersonId
                };

                switch (request.Field.ToLower())
                {
                    case "oath_tag":
                        var newOathTag = request.Value?.ToString();
                        if (string.IsNullOrWhiteSpace(newOathTag))
                        {
                            return Json(new InlineEditResponse { Success = false, Message = "OATH Tag cannot be empty" });
                        }
                        
                        // Check if OATH Tag already exists (excluding current equipment)
                        var existingEquipment = await _context.Equipment
                            .FirstOrDefaultAsync(e => e.OATH_Tag == newOathTag && e.Id != equipment.Id);
                        if (existingEquipment != null)
                        {
                            return Json(new InlineEditResponse { Success = false, Message = "OATH Tag already exists" });
                        }
                        
                        equipment.OATH_Tag = newOathTag;
                        await LogEquipmentAction(equipment.Id, "OATH Tag Updated", $"OATH Tag set to {request.Value}");
                        break;

                    case "assigned_user_name":
                        equipment.Assigned_User_Name = request.Value?.ToString();
                        await LogEquipmentAction(equipment.Id, "Assigned User Updated", $"Assigned user set to {request.Value}");
                        break;

                    case "assetcategoryid":
                        // Find the category by name
                        var categoryName = request.Value?.ToString();
                        if (string.IsNullOrWhiteSpace(categoryName))
                        {
                            return Json(new InlineEditResponse { Success = false, Message = "Category cannot be empty" });
                        }
                        
                        var category = await _context.AssetCategories
                            .FirstOrDefaultAsync(c => c.Name == categoryName && c.IsActive);
                        if (category == null)
                        {
                            return Json(new InlineEditResponse { Success = false, Message = $"Category '{categoryName}' not found" });
                        }
                        
                        equipment.AssetCategoryId = category.Id;
                        await LogEquipmentAction(equipment.Id, "Category Updated", $"Category set to {categoryName}");
                        break;

                    case "department":
                        equipment.Department = request.Value?.ToString();
                        await LogEquipmentAction(equipment.Id, "Department Updated", $"Department set to {request.Value}");
                        break;

                    case "currentlocationid":
                        // Find the location by name
                        var locationName = request.Value?.ToString();
                        if (string.IsNullOrWhiteSpace(locationName))
                        {
                            return Json(new InlineEditResponse { Success = false, Message = "Location cannot be empty" });
                        }
                        
                        var location = await _context.Locations
                            .FirstOrDefaultAsync(l => l.Name == locationName && l.IsActive);
                        if (location == null)
                        {
                            return Json(new InlineEditResponse { Success = false, Message = $"Location '{locationName}' not found" });
                        }
                        
                        equipment.CurrentLocationId = location.Id;
                        await LogEquipmentAction(equipment.Id, "Location Updated", $"Location set to {locationName}");
                        break;

                    case "currentfloorplanid":
                        // Find the floor plan by name
                        var floorName = request.Value?.ToString();
                        if (string.IsNullOrWhiteSpace(floorName))
                        {
                            return Json(new InlineEditResponse { Success = false, Message = "Floor cannot be empty" });
                        }
                        
                        var floorPlan = await _context.FloorPlans
                            .FirstOrDefaultAsync(f => f.FloorName == floorName && f.IsActive);
                        if (floorPlan == null)
                        {
                            return Json(new InlineEditResponse { Success = false, Message = $"Floor '{floorName}' not found" });
                        }
                        
                        equipment.CurrentFloorPlanId = floorPlan.Id;
                        await LogEquipmentAction(equipment.Id, "Floor Updated", $"Floor set to {floorName}");
                        break;

                    case "currentdeskid":
                        // Find the desk by name
                        var deskName = request.Value?.ToString();
                        if (string.IsNullOrWhiteSpace(deskName))
                        {
                            return Json(new InlineEditResponse { Success = false, Message = "Desk cannot be empty" });
                        }
                        
                        var desk = await _context.Desks
                            .FirstOrDefaultAsync(d => d.DeskName == deskName && d.IsActive);
                        if (desk == null)
                        {
                            return Json(new InlineEditResponse { Success = false, Message = $"Desk '{deskName}' not found" });
                        }
                        
                        equipment.CurrentDeskId = desk.Id;
                        await LogEquipmentAction(equipment.Id, "Desk Updated", $"Desk set to {deskName}");
                        break;

                    case "model":
                        equipment.Model = request.Value?.ToString();
                        await LogEquipmentAction(equipment.Id, "Model Updated", $"Model set to {request.Value}");
                        break;

                    case "serial_number":
                        equipment.Serial_Number = request.Value?.ToString();
                        await LogEquipmentAction(equipment.Id, "Serial Number Updated", $"Serial number set to {request.Value}");
                        break;

                    case "manufacturer":
                        equipment.Manufacturer = request.Value?.ToString();
                        await LogEquipmentAction(equipment.Id, "Manufacturer Updated", $"Manufacturer set to {request.Value}");
                        break;

                    case "purchasedate":
                        if (DateTime.TryParse(request.Value?.ToString(), out DateTime purchaseDate))
                        {
                            equipment.PurchaseDate = purchaseDate;
                            await LogEquipmentAction(equipment.Id, "Purchase Date Updated", $"Purchase date set to {purchaseDate:MM/dd/yyyy}");
                        }
                        else
                        {
                            return Json(new InlineEditResponse { Success = false, Message = "Invalid date format. Please use MM/dd/yyyy" });
                        }
                        break;

                    case "currentstatusid":
                        if (int.TryParse(request.Value?.ToString(), out int statusId))
                        {
                            var status = await _context.AssetStatuses.FindAsync(statusId);
                            if (status != null)
                            {
                                equipment.CurrentStatusId = statusId;
                                await LogEquipmentAction(equipment.Id, "Status Changed", $"Status set to {status.Name}");
                            }
                            else
                            {
                                return Json(new InlineEditResponse { Success = false, Message = "Invalid status" });
                            }
                        }
                        else
                        {
                            return Json(new InlineEditResponse { Success = false, Message = "Invalid status value" });
                        }
                        break;

                    case "netname":
                        if (equipment.TechnologyConfiguration == null)
                        {
                            equipment.TechnologyConfiguration = new TechnologyConfiguration
                            {
                                EquipmentId = equipment.Id,
                                LastUpdated = DateTime.UtcNow,
                                UpdatedBy = User.Identity?.Name ?? "System"
                            };
                            _context.TechnologyConfigurations.Add(equipment.TechnologyConfiguration);
                        }
                        equipment.TechnologyConfiguration.NetName = request.Value?.ToString();
                        equipment.TechnologyConfiguration.LastUpdated = DateTime.UtcNow;
                        equipment.TechnologyConfiguration.UpdatedBy = User.Identity?.Name ?? "System";
                        await LogEquipmentAction(equipment.Id, "Net Name Updated", $"Net name set to {request.Value}");
                        break;

                    default:
                        return Json(new InlineEditResponse { Success = false, Message = $"Field '{request.Field}' is not editable" });
                }

                equipment.UpdatedAt = DateTime.UtcNow;
                equipment.UpdatedBy = User.Identity?.Name ?? "System";

                await _context.SaveChangesAsync();
                await LogEquipmentChanges(originalEquipment, equipment);

                return Json(new InlineEditResponse 
                { 
                    Success = true, 
                    Message = "Field updated successfully",
                    NewValue = request.Value
                });
            }
            catch (Exception ex)
            {
                return Json(new InlineEditResponse { Success = false, Message = $"Error updating field: {ex.Message}" });
            }
        }

        // Bulk Actions
        [HttpPost("BulkSetStatus")]
        public async Task<IActionResult> BulkSetStatus([FromBody] BulkActionRequest request)
        {
            try
            {
                if (request.Ids == null || !request.Ids.Any())
                {
                    return Json(new { Success = false, Message = "No equipment selected" });
                }

                if (!int.TryParse(request.Value, out int statusId))
                {
                    return Json(new { Success = false, Message = "Invalid status value" });
                }

                var status = await _context.AssetStatuses.FindAsync(statusId);
                if (status == null)
                {
                    return Json(new { Success = false, Message = "Status not found" });
                }

                var equipmentList = await _context.Equipment
                    .Where(e => request.Ids.Contains(e.Id))
                    .ToListAsync();

                foreach (var equipment in equipmentList)
                {
                    equipment.CurrentStatusId = statusId;
                    equipment.UpdatedAt = DateTime.UtcNow;
                    equipment.UpdatedBy = User.Identity?.Name ?? "System";
                    await LogEquipmentAction(equipment.Id, "Bulk Status Change", $"Status set to {status.Name}");
                }

                await _context.SaveChangesAsync();

                return Json(new { Success = true, Message = $"Updated status for {equipmentList.Count} equipment items" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = $"Error updating status: {ex.Message}" });
            }
        }

        [HttpPost("ExportSelected")]
        public async Task<IActionResult> ExportSelected([FromBody] BulkActionRequest request)
        {
            try
            {
                if (request.Ids == null || !request.Ids.Any())
                {
                    return Json(new { Success = false, Message = "No equipment selected" });
                }

                var equipmentList = await _context.Equipment
                    .Include(e => e.AssetCategory)
                    .Include(e => e.CurrentStatus)
                    .Include(e => e.CurrentLocation)
                    .Include(e => e.CurrentFloorPlan)
                    .Include(e => e.TechnologyConfiguration)
                    .Where(e => request.Ids.Contains(e.Id))
                    .ToListAsync();

                // Generate CSV content
                var csvContent = "OATH Tag,Serial Number,Model,Category,Status,Location,Assigned To,Net Name,Manufacturer,Purchase Date\n";
                
                foreach (var equipment in equipmentList)
                {
                    csvContent += $"\"{equipment.OATH_Tag}\",\"{equipment.Serial_Number ?? ""}\",\"{equipment.Model ?? ""}\",\"{equipment.AssetCategory?.Name ?? ""}\",\"{equipment.CurrentStatus?.Name ?? ""}\",\"{equipment.CurrentLocation?.Name ?? ""}\",\"{equipment.Assigned_User_Name ?? ""}\",\"{equipment.TechnologyConfiguration?.NetName ?? ""}\",\"{equipment.Manufacturer ?? ""}\",\"{equipment.PurchaseDate?.ToString("MM/dd/yyyy") ?? ""}\"\n";
                }

                var fileName = $"equipment_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = $"Error exporting data: {ex.Message}" });
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



    public class BulkActionRequest
    {
        public List<int>? Ids { get; set; }
        public string? Value { get; set; }
    }
}
