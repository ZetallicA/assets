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
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var techConfigs = from tc in _context.TechnologyConfigurations
                             .Include(tc => tc.Equipment)
                             select tc;

            if (!string.IsNullOrEmpty(searchString))
            {
                techConfigs = techConfigs.Where(tc => 
                    tc.Equipment.OATH_Tag.Contains(searchString) ||
                    tc.NetName.Contains(searchString) ||
                    tc.IPv4Address.Contains(searchString) ||
                    tc.MACAddress.Contains(searchString) ||
                    tc.PhoneNumber.Contains(searchString));
            }

            techConfigs = techConfigs.OrderBy(tc => tc.Equipment.OATH_Tag);

            return View(await techConfigs.AsNoTracking().ToListAsync());
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
    }
}
