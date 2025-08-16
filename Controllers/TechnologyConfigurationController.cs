using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;

namespace AssetManagement.Controllers
{
    public class TechnologyConfigurationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TechnologyConfigurationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TechnologyConfiguration
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? pageNumber, int? pageSize,
            string? equipmentFilter, string? wallPortFilter, string? netNameFilter, string? ipv4Filter, 
            string? macFilter, string? switchPortFilter, string? phoneFilter)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentPageSize"] = pageSize ?? 10;

            // Set up sorting parameters
            ViewData["EquipmentSortParm"] = String.IsNullOrEmpty(sortOrder) ? "equipment_desc" : "";
            ViewData["WallPortSortParm"] = sortOrder == "wallport" ? "wallport_desc" : "wallport";
            ViewData["NetNameSortParm"] = sortOrder == "netname" ? "netname_desc" : "netname";
            ViewData["IPv4SortParm"] = sortOrder == "ipv4" ? "ipv4_desc" : "ipv4";
            ViewData["MACSortParm"] = sortOrder == "mac" ? "mac_desc" : "mac";
            ViewData["SwitchPortSortParm"] = sortOrder == "switchport" ? "switchport_desc" : "switchport";
            ViewData["PhoneSortParm"] = sortOrder == "phone" ? "phone_desc" : "phone";

            // Set up filter parameters
            ViewData["EquipmentFilter"] = equipmentFilter;
            ViewData["WallPortFilter"] = wallPortFilter;
            ViewData["NetNameFilter"] = netNameFilter;
            ViewData["IPv4Filter"] = ipv4Filter;
            ViewData["MACFilter"] = macFilter;
            ViewData["SwitchPortFilter"] = switchPortFilter;
            ViewData["PhoneFilter"] = phoneFilter;

            var techConfigs = from tc in _context.TechnologyConfigurations
                             .Include(tc => tc.Equipment)
                             select tc;

            // Apply master search filter across all columns
            if (!string.IsNullOrEmpty(searchString))
            {
                var searchTerm = searchString.ToLower();
                techConfigs = techConfigs.Where(tc => 
                    (tc.Equipment.OATH_Tag != null && tc.Equipment.OATH_Tag.ToLower().Contains(searchTerm)) ||
                    (tc.Equipment.Model != null && tc.Equipment.Model.ToLower().Contains(searchTerm)) ||
                    (tc.NetName != null && tc.NetName.ToLower().Contains(searchTerm)) ||
                    (tc.IPv4Address != null && tc.IPv4Address.ToLower().Contains(searchTerm)) ||
                    (tc.MACAddress != null && tc.MACAddress.ToLower().Contains(searchTerm)) ||
                    (tc.WallPort != null && tc.WallPort.ToLower().Contains(searchTerm)) ||
                    (tc.SwitchName != null && tc.SwitchName.ToLower().Contains(searchTerm)) ||
                    (tc.SwitchPort != null && tc.SwitchPort.ToLower().Contains(searchTerm)) ||
                    (tc.PhoneNumber != null && tc.PhoneNumber.ToLower().Contains(searchTerm)) ||
                    (tc.Extension != null && tc.Extension.ToLower().Contains(searchTerm)) ||
                    (tc.IMEI != null && tc.IMEI.ToLower().Contains(searchTerm)) ||
                    (tc.Vendor != null && tc.Vendor.ToLower().Contains(searchTerm)));
            }

            // Apply column filters
            if (!String.IsNullOrEmpty(equipmentFilter))
            {
                techConfigs = techConfigs.Where(tc => tc.Equipment.OATH_Tag == equipmentFilter);
            }

            if (!String.IsNullOrEmpty(wallPortFilter))
            {
                techConfigs = techConfigs.Where(tc => tc.WallPort == wallPortFilter);
            }

            if (!String.IsNullOrEmpty(netNameFilter))
            {
                techConfigs = techConfigs.Where(tc => tc.NetName == netNameFilter);
            }

            if (!String.IsNullOrEmpty(ipv4Filter))
            {
                techConfigs = techConfigs.Where(tc => tc.IPv4Address == ipv4Filter);
            }

            if (!String.IsNullOrEmpty(macFilter))
            {
                techConfigs = techConfigs.Where(tc => tc.MACAddress == macFilter);
            }

            if (!String.IsNullOrEmpty(switchPortFilter))
            {
                techConfigs = techConfigs.Where(tc => tc.SwitchPort == switchPortFilter);
            }

            if (!String.IsNullOrEmpty(phoneFilter))
            {
                techConfigs = techConfigs.Where(tc => tc.PhoneNumber == phoneFilter);
            }

            // Apply sorting
            techConfigs = sortOrder switch
            {
                "equipment_desc" => techConfigs.OrderByDescending(tc => tc.Equipment.OATH_Tag),
                "wallport" => techConfigs.OrderBy(tc => tc.WallPort),
                "wallport_desc" => techConfigs.OrderByDescending(tc => tc.WallPort),
                "netname" => techConfigs.OrderBy(tc => tc.NetName),
                "netname_desc" => techConfigs.OrderByDescending(tc => tc.NetName),
                "ipv4" => techConfigs.OrderBy(tc => tc.IPv4Address),
                "ipv4_desc" => techConfigs.OrderByDescending(tc => tc.IPv4Address),
                "mac" => techConfigs.OrderBy(tc => tc.MACAddress),
                "mac_desc" => techConfigs.OrderByDescending(tc => tc.MACAddress),
                "switchport" => techConfigs.OrderBy(tc => tc.SwitchPort),
                "switchport_desc" => techConfigs.OrderByDescending(tc => tc.SwitchPort),
                "phone" => techConfigs.OrderBy(tc => tc.PhoneNumber),
                "phone_desc" => techConfigs.OrderByDescending(tc => tc.PhoneNumber),
                _ => techConfigs.OrderBy(tc => tc.Equipment.OATH_Tag),
            };

            // Get filter options for dropdowns
            ViewData["EquipmentOptions"] = await _context.TechnologyConfigurations
                .Include(tc => tc.Equipment)
                .Where(tc => tc.Equipment != null && !String.IsNullOrEmpty(tc.Equipment.OATH_Tag))
                .Select(tc => tc.Equipment.OATH_Tag)
                .Distinct()
                .OrderBy(o => o)
                .ToListAsync();

            ViewData["WallPortOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.WallPort))
                .Select(tc => tc.WallPort)
                .Distinct()
                .OrderBy(o => o)
                .ToListAsync();

            ViewData["NetNameOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.NetName))
                .Select(tc => tc.NetName)
                .Distinct()
                .OrderBy(o => o)
                .ToListAsync();

            ViewData["IPv4Options"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.IPv4Address))
                .Select(tc => tc.IPv4Address)
                .Distinct()
                .OrderBy(o => o)
                .ToListAsync();

            ViewData["MACOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.MACAddress))
                .Select(tc => tc.MACAddress)
                .Distinct()
                .OrderBy(o => o)
                .ToListAsync();

            ViewData["SwitchPortOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.SwitchPort))
                .Select(tc => tc.SwitchPort)
                .Distinct()
                .OrderBy(o => o)
                .ToListAsync();

            ViewData["PhoneOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.PhoneNumber))
                .Select(tc => tc.PhoneNumber)
                .Distinct()
                .OrderBy(o => o)
                .ToListAsync();

            // Apply pagination
            int currentPageSize = pageSize ?? 10;
            int currentPageNumber = pageNumber ?? 1;

            var totalCount = await techConfigs.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)currentPageSize);

            var pagedTechConfigs = await techConfigs
                .Skip((currentPageNumber - 1) * currentPageSize)
                .Take(currentPageSize)
                .AsNoTracking()
                .ToListAsync();

            ViewData["TotalCount"] = totalCount;
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = currentPageNumber;

            return View(pagedTechConfigs);
        }

        // GET: TechnologyConfiguration/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var techConfig = await _context.TechnologyConfigurations
                .Include(tc => tc.Equipment)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (techConfig == null)
            {
                return NotFound();
            }

            return View(techConfig);
        }

        // GET: TechnologyConfiguration/Create
        public async Task<IActionResult> Create(int? equipmentId)
        {
            ViewData["EquipmentId"] = await GetEquipmentSelectList(equipmentId);
            
            var model = new TechnologyConfiguration();
            if (equipmentId.HasValue)
            {
                model.EquipmentId = equipmentId.Value;
            }
            
            return View(model);
        }

        // POST: TechnologyConfiguration/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EquipmentId,NetName,IPv4Address,IPv6Address,MACAddress,WallPort,SwitchPort,SwitchName,VLAN,PhoneNumber,Extension,IMEI,SIMCardNumber,Vendor,ServicePlan,Domain,Workgroup,OperatingSystem,OSVersion,Antivirus,RemoteAccessTool,RemoteAccessID,BackupSolution,ConfigurationNotes")] TechnologyConfiguration technologyConfiguration)
        {
            if (ModelState.IsValid)
            {
                // Check if configuration already exists for this equipment
                var existingConfig = await _context.TechnologyConfigurations
                    .FirstOrDefaultAsync(tc => tc.EquipmentId == technologyConfiguration.EquipmentId);
                
                if (existingConfig != null)
                {
                    TempData["Error"] = "Technology configuration already exists for this equipment. Please edit the existing configuration.";
                    return RedirectToAction("Edit", new { id = existingConfig.Id });
                }

                technologyConfiguration.LastUpdated = DateTime.UtcNow;
                technologyConfiguration.UpdatedBy = User.Identity?.Name ?? "System";
                
                _context.Add(technologyConfiguration);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Technology configuration created successfully.";
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["EquipmentId"] = await GetEquipmentSelectList(technologyConfiguration.EquipmentId);
            return View(technologyConfiguration);
        }

        // GET: TechnologyConfiguration/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var technologyConfiguration = await _context.TechnologyConfigurations.FindAsync(id);
            if (technologyConfiguration == null)
            {
                return NotFound();
            }
            
            ViewData["EquipmentId"] = await GetEquipmentSelectList(technologyConfiguration.EquipmentId);
            return View(technologyConfiguration);
        }

        // POST: TechnologyConfiguration/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EquipmentId,NetName,IPv4Address,IPv6Address,MACAddress,WallPort,SwitchPort,SwitchName,VLAN,PhoneNumber,Extension,IMEI,SIMCardNumber,Vendor,ServicePlan,Domain,Workgroup,OperatingSystem,OSVersion,Antivirus,RemoteAccessTool,RemoteAccessID,BackupSolution,ConfigurationNotes")] TechnologyConfiguration technologyConfiguration)
        {
            if (id != technologyConfiguration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    technologyConfiguration.LastUpdated = DateTime.UtcNow;
                    technologyConfiguration.UpdatedBy = User.Identity?.Name ?? "System";
                    
                    _context.Update(technologyConfiguration);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Technology configuration updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TechnologyConfigurationExists(technologyConfiguration.Id))
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
            
            ViewData["EquipmentId"] = await GetEquipmentSelectList(technologyConfiguration.EquipmentId);
            return View(technologyConfiguration);
        }

        // GET: TechnologyConfiguration/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var technologyConfiguration = await _context.TechnologyConfigurations
                .Include(tc => tc.Equipment)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (technologyConfiguration == null)
            {
                return NotFound();
            }

            return View(technologyConfiguration);
        }

        // POST: TechnologyConfiguration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var technologyConfiguration = await _context.TechnologyConfigurations.FindAsync(id);
            if (technologyConfiguration != null)
            {
                _context.TechnologyConfigurations.Remove(technologyConfiguration);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Technology configuration deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }



        private bool TechnologyConfigurationExists(int id)
        {
            return _context.TechnologyConfigurations.Any(e => e.Id == id);
        }

        private async Task<Microsoft.AspNetCore.Mvc.Rendering.SelectList> GetEquipmentSelectList(int? selectedValue = null)
        {
            var equipment = await _context.Equipment
                .Where(e => e.IsActive)
                .OrderBy(e => e.OATH_Tag)
                .Select(e => new { e.Id, DisplayText = $"{e.OATH_Tag} - {e.Model}" })
                .ToListAsync();

            return new Microsoft.AspNetCore.Mvc.Rendering.SelectList(equipment, "Id", "DisplayText", selectedValue);
        }

        [HttpGet]
        public async Task<IActionResult> GetEquipmentSuggestions(string query = "")
        {
            if (string.IsNullOrEmpty(query))
            {
                // Return all equipment for dropdown
                var allEquipment = await _context.Equipment
                    .Where(e => e.IsActive)
                    .OrderBy(e => e.OATH_Tag)
                    .Select(e => new { 
                        Id = e.Id, 
                        Label = $"{e.OATH_Tag} - {e.Model}",
                        Value = e.Id
                    })
                    .ToListAsync();

                return Json(allEquipment);
            }

            if (query.Length < 2)
            {
                return Json(new List<object>());
            }

            var suggestions = await _context.Equipment
                .Where(e => e.IsActive && (e.OATH_Tag.Contains(query) || 
                           e.Model.Contains(query) || 
                           e.Serial_Number.Contains(query)))
                .OrderBy(e => e.OATH_Tag)
                .Select(e => new { 
                    Id = e.Id, 
                    Label = $"{e.OATH_Tag} - {e.Model}",
                    Value = e.Id
                })
                .Take(15)
                .ToListAsync();

            return Json(suggestions);
        }

        // GET: TechnologyConfiguration/SearchSuggestions
        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 2)
            {
                return Json(new List<string>());
            }

            var suggestions = new List<string>();
            var searchTerm = term.ToLower();

            // Get suggestions from all relevant fields
            var equipmentSuggestions = await _context.TechnologyConfigurations
                .Include(tc => tc.Equipment)
                .Where(tc => tc.Equipment.OATH_Tag != null && tc.Equipment.OATH_Tag.ToLower().Contains(searchTerm))
                .Select(tc => tc.Equipment.OATH_Tag)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .Take(5)
                .ToListAsync();

            var modelSuggestions = await _context.TechnologyConfigurations
                .Include(tc => tc.Equipment)
                .Where(tc => tc.Equipment.Model != null && tc.Equipment.Model.ToLower().Contains(searchTerm))
                .Select(tc => tc.Equipment.Model)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .Take(5)
                .ToListAsync();

            var netNameSuggestions = await _context.TechnologyConfigurations
                .Where(tc => tc.NetName != null && tc.NetName.ToLower().Contains(searchTerm))
                .Select(tc => tc.NetName)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .Take(5)
                .ToListAsync();

            var ipSuggestions = await _context.TechnologyConfigurations
                .Where(tc => tc.IPv4Address != null && tc.IPv4Address.ToLower().Contains(searchTerm))
                .Select(tc => tc.IPv4Address)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .Take(5)
                .ToListAsync();

            var macSuggestions = await _context.TechnologyConfigurations
                .Where(tc => tc.MACAddress != null && tc.MACAddress.ToLower().Contains(searchTerm))
                .Select(tc => tc.MACAddress)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .Take(5)
                .ToListAsync();

            var wallPortSuggestions = await _context.TechnologyConfigurations
                .Where(tc => tc.WallPort != null && tc.WallPort.ToLower().Contains(searchTerm))
                .Select(tc => tc.WallPort)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .Take(5)
                .ToListAsync();

            var phoneSuggestions = await _context.TechnologyConfigurations
                .Where(tc => tc.PhoneNumber != null && tc.PhoneNumber.ToLower().Contains(searchTerm))
                .Select(tc => tc.PhoneNumber)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .Take(5)
                .ToListAsync();

            // Combine all suggestions and remove duplicates
            suggestions.AddRange(equipmentSuggestions.Cast<string>());
            suggestions.AddRange(modelSuggestions.Cast<string>());
            suggestions.AddRange(netNameSuggestions.Cast<string>());
            suggestions.AddRange(ipSuggestions.Cast<string>());
            suggestions.AddRange(macSuggestions.Cast<string>());
            suggestions.AddRange(wallPortSuggestions.Cast<string>());
            suggestions.AddRange(phoneSuggestions.Cast<string>());

            // Remove duplicates and limit to 10 suggestions
            var uniqueSuggestions = suggestions.Distinct().Take(10).ToList();

            return Json(uniqueSuggestions);
        }

        // POST: TechnologyConfiguration/UpdateField
        [HttpPost]
        public async Task<IActionResult> UpdateField(int id, string field, string value)
        {
            try
            {
                var techConfig = await _context.TechnologyConfigurations
                    .Include(tc => tc.Equipment)
                    .FirstOrDefaultAsync(tc => tc.Id == id);

                if (techConfig == null)
                {
                    return Json(new { success = false, message = "Technology configuration not found." });
                }

                // Handle different field types
                switch (field)
                {
                    case "WallPort":
                        techConfig.WallPort = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
                        break;
                    case "NetName":
                        techConfig.NetName = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
                        break;
                    case "IPv4Address":
                        techConfig.IPv4Address = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
                        break;
                    case "MACAddress":
                        techConfig.MACAddress = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
                        break;
                    case "SwitchPort":
                        techConfig.SwitchPort = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
                        break;
                    case "PhoneNumber":
                        techConfig.PhoneNumber = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
                        break;
                    case "Equipment.OATH_Tag":
                        if (techConfig.Equipment != null)
                        {
                            techConfig.Equipment.OATH_Tag = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
                        }
                        break;
                    case "Equipment.Model":
                        if (techConfig.Equipment != null)
                        {
                            techConfig.Equipment.Model = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
                        }
                        break;
                    default:
                        return Json(new { success = false, message = "Invalid field specified." });
                }

                techConfig.LastUpdated = DateTime.Now;
                techConfig.UpdatedBy = "User";

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Field updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error updating field: {ex.Message}" });
            }
        }
    }
}
