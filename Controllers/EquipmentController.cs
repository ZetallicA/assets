using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

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
            string? deskNumberFilter, string? manufacturerFilter, string? purchaseDateFilter, string? assetTagFilter, string? serviceTagFilter, string? ipAddressFilter, string? osVersionFilter, string? phoneNumberFilter,
            string? purchasePriceFilter, string? warrantyEndFilter, string? notesFilter, string? macAddressFilter, string? wallPortFilter, string? switchNameFilter, string? switchPortFilter, string? extensionFilter, string? imeiFilter, string? simCardFilter, string? configNotesFilter)
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
            ViewData["AssetTagFilter"] = assetTagFilter;
            ViewData["ServiceTagFilter"] = serviceTagFilter;
            ViewData["IPAddressFilter"] = ipAddressFilter;
            ViewData["OSVersionFilter"] = osVersionFilter;
            ViewData["PhoneNumberFilter"] = phoneNumberFilter;
            ViewData["PurchasePriceFilter"] = purchasePriceFilter;
            ViewData["WarrantyEndFilter"] = warrantyEndFilter;
            ViewData["NotesFilter"] = notesFilter;
            ViewData["MACAddressFilter"] = macAddressFilter;
            ViewData["WallPortFilter"] = wallPortFilter;
            ViewData["SwitchNameFilter"] = switchNameFilter;
            ViewData["SwitchPortFilter"] = switchPortFilter;
            ViewData["ExtensionFilter"] = extensionFilter;
            ViewData["IMEIFilter"] = imeiFilter;
            ViewData["SIMCardFilter"] = simCardFilter;
            ViewData["ConfigNotesFilter"] = configNotesFilter;
            
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

            // Apply category filter (support multiple values)
            if (!String.IsNullOrEmpty(categoryFilter))
            {
                var categories = categoryFilter.Split(',').Select(c => c.Trim()).ToList();
                equipment = equipment.Where(e => e.AssetCategory != null && categories.Contains(e.AssetCategory.Name));
            }

            // Apply status filter (support multiple values)
            if (!String.IsNullOrEmpty(statusFilter))
            {
                var statuses = statusFilter.Split(',').Select(s => s.Trim()).ToList();
                equipment = equipment.Where(e => e.CurrentStatus != null && statuses.Contains(e.CurrentStatus.Name));
            }

            // Apply department filter (support multiple values)
            if (!String.IsNullOrEmpty(departmentFilter))
            {
                var departments = departmentFilter.Split(',').Select(d => d.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Department) && departments.Contains(e.Department));
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

            // Apply Location filter (Building only) - support multiple values
            if (!String.IsNullOrEmpty(locationFilter))
            {
                var locations = locationFilter.Split(',').Select(l => l.Trim()).ToList();
                equipment = equipment.Where(e => e.CurrentLocation != null && locations.Any(loc => e.CurrentLocation.Name.StartsWith(loc)));
            }

            // Apply Floor filter - support multiple values
            if (!String.IsNullOrEmpty(floorFilter))
            {
                var floors = floorFilter.Split(',').Select(f => f.Trim()).ToList();
                equipment = equipment.Where(e => e.CurrentFloorPlan != null && floors.Contains(e.CurrentFloorPlan.FloorName));
            }

            // Apply Net Name filter (support multiple values)
            if (!String.IsNullOrEmpty(netNameFilter))
            {
                var netNames = netNameFilter.Split(',').Select(n => n.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.NetName) && 
                                                netNames.Contains(e.TechnologyConfiguration.NetName));
            }

            // Apply Model filter (support multiple values)
            if (!String.IsNullOrEmpty(modelFilter))
            {
                var models = modelFilter.Split(',').Select(m => m.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Model) && models.Contains(e.Model));
            }

            // Apply Serial Number filter (support multiple values)
            if (!String.IsNullOrEmpty(serialNumberFilter))
            {
                var serialNumbers = serialNumberFilter.Split(',').Select(s => s.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Serial_Number) && serialNumbers.Contains(e.Serial_Number));
            }

            // Apply Desk Number filter (support multiple values)
            if (!String.IsNullOrEmpty(deskNumberFilter))
            {
                var deskNumbers = deskNumberFilter.Split(',').Select(d => d.Trim()).ToList();
                equipment = equipment.Where(e => e.CurrentDesk != null && deskNumbers.Contains(e.CurrentDesk.DeskName));
            }

            // Apply Manufacturer filter (support multiple values)
            if (!String.IsNullOrEmpty(manufacturerFilter))
            {
                var manufacturers = manufacturerFilter.Split(',').Select(m => m.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Manufacturer) && manufacturers.Contains(e.Manufacturer));
            }

            // Apply Purchase Date filter (support multiple values)
            if (!String.IsNullOrEmpty(purchaseDateFilter))
            {
                var purchaseDates = purchaseDateFilter.Split(',').Select(p => p.Trim()).ToList();
                equipment = equipment.Where(e => e.PurchaseDate.HasValue && purchaseDates.Contains(e.PurchaseDate.Value.ToString("MM/dd/yyyy")));
            }

            // Apply Asset Tag filter (support multiple values)
            if (!String.IsNullOrEmpty(assetTagFilter))
            {
                var assetTags = assetTagFilter.Split(',').Select(a => a.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Asset_Tag) && assetTags.Contains(e.Asset_Tag));
            }

            // Apply Service Tag filter (support multiple values)
            if (!String.IsNullOrEmpty(serviceTagFilter))
            {
                var serviceTags = serviceTagFilter.Split(',').Select(s => s.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Service_Tag) && serviceTags.Contains(e.Service_Tag));
            }

            // Apply IP Address filter (support multiple values)
            if (!String.IsNullOrEmpty(ipAddressFilter))
            {
                var ipAddresses = ipAddressFilter.Split(',').Select(i => i.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.IP_Address) && ipAddresses.Contains(e.IP_Address));
            }

            // Apply OS Version filter (support multiple values)
            if (!String.IsNullOrEmpty(osVersionFilter))
            {
                var osVersions = osVersionFilter.Split(',').Select(o => o.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.OS_Version) && osVersions.Contains(e.OS_Version));
            }

            // Apply Phone Number filter (support multiple values)
            if (!String.IsNullOrEmpty(phoneNumberFilter))
            {
                var phoneNumbers = phoneNumberFilter.Split(',').Select(p => p.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.PhoneNumber) && 
                                                phoneNumbers.Contains(e.TechnologyConfiguration.PhoneNumber));
            }

            // Apply Purchase Price filter (support multiple values)
            if (!String.IsNullOrEmpty(purchasePriceFilter))
            {
                var purchasePrices = purchasePriceFilter.Split(',').Select(p => p.Trim()).ToList();
                equipment = equipment.Where(e => e.PurchasePrice.HasValue && purchasePrices.Contains(e.PurchasePrice.Value.ToString("C")));
            }

            // Apply Warranty End filter (support multiple values)
            if (!String.IsNullOrEmpty(warrantyEndFilter))
            {
                var warrantyEnds = warrantyEndFilter.Split(',').Select(w => w.Trim()).ToList();
                equipment = equipment.Where(e => e.WarrantyEndDate.HasValue && warrantyEnds.Contains(e.WarrantyEndDate.Value.ToString("MM/dd/yyyy")));
            }

            // Apply Notes filter (support multiple values)
            if (!String.IsNullOrEmpty(notesFilter))
            {
                var notes = notesFilter.Split(',').Select(n => n.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Notes) && notes.Contains(e.Notes));
            }

            // Apply MAC Address filter (support multiple values)
            if (!String.IsNullOrEmpty(macAddressFilter))
            {
                var macAddresses = macAddressFilter.Split(',').Select(m => m.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.MACAddress) && 
                                                macAddresses.Contains(e.TechnologyConfiguration.MACAddress));
            }

            // Apply Wall Port filter (support multiple values)
            if (!String.IsNullOrEmpty(wallPortFilter))
            {
                var wallPorts = wallPortFilter.Split(',').Select(w => w.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.WallPort) && 
                                                wallPorts.Contains(e.TechnologyConfiguration.WallPort));
            }

            // Apply Switch Name filter (support multiple values)
            if (!String.IsNullOrEmpty(switchNameFilter))
            {
                var switchNames = switchNameFilter.Split(',').Select(s => s.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.SwitchName) && 
                                                switchNames.Contains(e.TechnologyConfiguration.SwitchName));
            }

            // Apply Switch Port filter (support multiple values)
            if (!String.IsNullOrEmpty(switchPortFilter))
            {
                var switchPorts = switchPortFilter.Split(',').Select(s => s.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.SwitchPort) && 
                                                switchPorts.Contains(e.TechnologyConfiguration.SwitchPort));
            }

            // Apply Extension filter (support multiple values)
            if (!String.IsNullOrEmpty(extensionFilter))
            {
                var extensions = extensionFilter.Split(',').Select(e => e.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.Extension) && 
                                                extensions.Contains(e.TechnologyConfiguration.Extension));
            }

            // Apply IMEI filter (support multiple values)
            if (!String.IsNullOrEmpty(imeiFilter))
            {
                var imeis = imeiFilter.Split(',').Select(i => i.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.IMEI) && 
                                                imeis.Contains(e.TechnologyConfiguration.IMEI));
            }

            // Apply SIM Card filter (support multiple values)
            if (!String.IsNullOrEmpty(simCardFilter))
            {
                var simCards = simCardFilter.Split(',').Select(s => s.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.SIMCardNumber) && 
                                                simCards.Contains(e.TechnologyConfiguration.SIMCardNumber));
            }

            // Apply Config Notes filter (support multiple values)
            if (!String.IsNullOrEmpty(configNotesFilter))
            {
                var configNotes = configNotesFilter.Split(',').Select(c => c.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.ConfigurationNotes) && 
                                                configNotes.Contains(e.TechnologyConfiguration.ConfigurationNotes));
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
            var purchaseDateOptions = await _context.Equipment
                .Where(e => e.PurchaseDate.HasValue)
                .Select(e => e.PurchaseDate.Value)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();
            
            ViewData["PurchaseDateOptions"] = purchaseDateOptions
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
            ViewData["PhoneNumberOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.PhoneNumber))
                .Select(tc => tc.PhoneNumber)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();

            // Get Purchase Price options
            var purchasePriceOptions = await _context.Equipment
                .Where(e => e.PurchasePrice.HasValue)
                .Select(e => e.PurchasePrice.Value)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();
            
            ViewData["PurchasePriceOptions"] = purchasePriceOptions
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

            // Get MAC Address options
            ViewData["MACAddressOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.MACAddress))
                .Select(tc => tc.MACAddress)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();

            // Get Wall Port options
            ViewData["WallPortOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.WallPort))
                .Select(tc => tc.WallPort)
                .Distinct()
                .OrderBy(w => w)
                .ToListAsync();

            // Get Switch Name options
            ViewData["SwitchNameOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.SwitchName))
                .Select(tc => tc.SwitchName)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            // Get Switch Port options
            ViewData["SwitchPortOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.SwitchPort))
                .Select(tc => tc.SwitchPort)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            // Get Extension options
            ViewData["ExtensionOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.Extension))
                .Select(tc => tc.Extension)
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();

            // Get IMEI options
            ViewData["IMEIOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.IMEI))
                .Select(tc => tc.IMEI)
                .Distinct()
                .OrderBy(i => i)
                .ToListAsync();

            // Get SIM Card options
            ViewData["SIMCardOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.SIMCardNumber))
                .Select(tc => tc.SIMCardNumber)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            // Get Config Notes options (first 50 characters for filtering)
            ViewData["ConfigNotesOptions"] = await _context.TechnologyConfigurations
                .Where(tc => !String.IsNullOrEmpty(tc.ConfigurationNotes))
                .Select(tc => tc.ConfigurationNotes.Length > 50 ? tc.ConfigurationNotes.Substring(0, 50) + "..." : tc.ConfigurationNotes)
                .Distinct()
                .OrderBy(c => c)
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

        // GET: Equipment/Export
        public async Task<IActionResult> Export(string searchString, string sortOrder, 
            string? categoryFilter, string? statusFilter, string? departmentFilter, 
            string? oathTagFilter, string? assignedToFilter, string? locationFilter, string? floorFilter, string? netNameFilter, string? modelFilter, string? serialNumberFilter,
            string? deskNumberFilter, string? manufacturerFilter, string? purchaseDateFilter, string? assetTagFilter, string? serviceTagFilter, string? ipAddressFilter, string? osVersionFilter, string? phoneNumberFilter,
            string? purchasePriceFilter, string? warrantyEndFilter, string? notesFilter, string? macAddressFilter, string? wallPortFilter, string? switchNameFilter, string? switchPortFilter, string? extensionFilter, string? imeiFilter, string? simCardFilter, string? configNotesFilter,
            string? visibleColumns = null)
        {
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Get filtered equipment data (same logic as Index method)
            var equipment = _context.Equipment
                .Include(e => e.AssetCategory)
                .Include(e => e.CurrentStatus)
                .Include(e => e.CurrentLocation)
                .Include(e => e.CurrentFloorPlan)
                .Include(e => e.CurrentDesk)
                .Include(e => e.TechnologyConfiguration)
                .AsQueryable();

            // Apply search filter
            if (!String.IsNullOrEmpty(searchString))
            {
                equipment = equipment.Where(e => e.OATH_Tag.Contains(searchString) ||
                                                e.Serial_Number.Contains(searchString) ||
                                                e.Model.Contains(searchString) ||
                                                e.Assigned_User_Name.Contains(searchString) ||
                                                e.Assigned_User_Email.Contains(searchString) ||
                                                (e.AssetCategory != null && e.AssetCategory.Name.Contains(searchString)) ||
                                                (e.CurrentLocation != null && e.CurrentLocation.Name.Contains(searchString)) ||
                                                (e.TechnologyConfiguration != null && e.TechnologyConfiguration.NetName.Contains(searchString)));
            }

            // Apply all filters (same logic as Index method)
            if (!String.IsNullOrEmpty(categoryFilter))
            {
                var categories = categoryFilter.Split(',').Select(c => c.Trim()).ToList();
                equipment = equipment.Where(e => e.AssetCategory != null && categories.Contains(e.AssetCategory.Name));
            }

            if (!String.IsNullOrEmpty(statusFilter))
            {
                var statuses = statusFilter.Split(',').Select(s => s.Trim()).ToList();
                equipment = equipment.Where(e => e.CurrentStatus != null && statuses.Contains(e.CurrentStatus.Name));
            }

            if (!String.IsNullOrEmpty(departmentFilter))
            {
                var departments = departmentFilter.Split(',').Select(d => d.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Department) && departments.Contains(e.Department));
            }

            if (!String.IsNullOrEmpty(oathTagFilter))
            {
                var oathTags = oathTagFilter.Split(',').Select(o => o.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.OATH_Tag) && oathTags.Contains(e.OATH_Tag));
            }

            if (!String.IsNullOrEmpty(assignedToFilter))
            {
                var assignedUsers = assignedToFilter.Split(',').Select(a => a.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Assigned_User_Name) && assignedUsers.Contains(e.Assigned_User_Name));
            }

            if (!String.IsNullOrEmpty(locationFilter))
            {
                var locations = locationFilter.Split(',').Select(l => l.Trim()).ToList();
                equipment = equipment.Where(e => e.CurrentLocation != null && locations.Contains(e.CurrentLocation.Name.Split(new string[] { " - " }, StringSplitOptions.None)[0]));
            }

            if (!String.IsNullOrEmpty(floorFilter))
            {
                var floors = floorFilter.Split(',').Select(f => f.Trim()).ToList();
                equipment = equipment.Where(e => e.CurrentFloorPlan != null && floors.Contains(e.CurrentFloorPlan.FloorName));
            }

            if (!String.IsNullOrEmpty(netNameFilter))
            {
                var netNames = netNameFilter.Split(',').Select(n => n.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.NetName) && 
                                                netNames.Contains(e.TechnologyConfiguration.NetName));
            }

            if (!String.IsNullOrEmpty(modelFilter))
            {
                var models = modelFilter.Split(',').Select(m => m.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Model) && models.Contains(e.Model));
            }

            if (!String.IsNullOrEmpty(serialNumberFilter))
            {
                var serialNumbers = serialNumberFilter.Split(',').Select(s => s.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Serial_Number) && serialNumbers.Contains(e.Serial_Number));
            }

            if (!String.IsNullOrEmpty(deskNumberFilter))
            {
                var deskNumbers = deskNumberFilter.Split(',').Select(d => d.Trim()).ToList();
                equipment = equipment.Where(e => e.CurrentDesk != null && deskNumbers.Contains(e.CurrentDesk.DeskName));
            }

            if (!String.IsNullOrEmpty(manufacturerFilter))
            {
                var manufacturers = manufacturerFilter.Split(',').Select(m => m.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Manufacturer) && manufacturers.Contains(e.Manufacturer));
            }

            if (!String.IsNullOrEmpty(purchaseDateFilter))
            {
                var purchaseDates = purchaseDateFilter.Split(',').Select(p => p.Trim()).ToList();
                equipment = equipment.Where(e => e.PurchaseDate.HasValue && purchaseDates.Contains(e.PurchaseDate.Value.ToString("MM/dd/yyyy")));
            }

            if (!String.IsNullOrEmpty(assetTagFilter))
            {
                var assetTags = assetTagFilter.Split(',').Select(a => a.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Asset_Tag) && assetTags.Contains(e.Asset_Tag));
            }

            if (!String.IsNullOrEmpty(serviceTagFilter))
            {
                var serviceTags = serviceTagFilter.Split(',').Select(s => s.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Service_Tag) && serviceTags.Contains(e.Service_Tag));
            }

            if (!String.IsNullOrEmpty(ipAddressFilter))
            {
                var ipAddresses = ipAddressFilter.Split(',').Select(i => i.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.IP_Address) && ipAddresses.Contains(e.IP_Address));
            }

            if (!String.IsNullOrEmpty(osVersionFilter))
            {
                var osVersions = osVersionFilter.Split(',').Select(o => o.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.OS_Version) && osVersions.Contains(e.OS_Version));
            }

            if (!String.IsNullOrEmpty(phoneNumberFilter))
            {
                var phoneNumbers = phoneNumberFilter.Split(',').Select(p => p.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.PhoneNumber) && 
                                                phoneNumbers.Contains(e.TechnologyConfiguration.PhoneNumber));
            }

            if (!String.IsNullOrEmpty(purchasePriceFilter))
            {
                var purchasePrices = purchasePriceFilter.Split(',').Select(p => p.Trim()).ToList();
                equipment = equipment.Where(e => e.PurchasePrice.HasValue && purchasePrices.Contains(e.PurchasePrice.Value.ToString("C")));
            }

            if (!String.IsNullOrEmpty(warrantyEndFilter))
            {
                var warrantyEnds = warrantyEndFilter.Split(',').Select(w => w.Trim()).ToList();
                equipment = equipment.Where(e => e.WarrantyEndDate.HasValue && warrantyEnds.Contains(e.WarrantyEndDate.Value.ToString("MM/dd/yyyy")));
            }

            if (!String.IsNullOrEmpty(notesFilter))
            {
                var notes = notesFilter.Split(',').Select(n => n.Trim()).ToList();
                equipment = equipment.Where(e => !String.IsNullOrEmpty(e.Notes) && notes.Contains(e.Notes));
            }

            if (!String.IsNullOrEmpty(macAddressFilter))
            {
                var macAddresses = macAddressFilter.Split(',').Select(m => m.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.MACAddress) && 
                                                macAddresses.Contains(e.TechnologyConfiguration.MACAddress));
            }

            if (!String.IsNullOrEmpty(wallPortFilter))
            {
                var wallPorts = wallPortFilter.Split(',').Select(w => w.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.WallPort) && 
                                                wallPorts.Contains(e.TechnologyConfiguration.WallPort));
            }

            if (!String.IsNullOrEmpty(switchNameFilter))
            {
                var switchNames = switchNameFilter.Split(',').Select(s => s.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.SwitchName) && 
                                                switchNames.Contains(e.TechnologyConfiguration.SwitchName));
            }

            if (!String.IsNullOrEmpty(switchPortFilter))
            {
                var switchPorts = switchPortFilter.Split(',').Select(s => s.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.SwitchPort) && 
                                                switchPorts.Contains(e.TechnologyConfiguration.SwitchPort));
            }

            if (!String.IsNullOrEmpty(extensionFilter))
            {
                var extensions = extensionFilter.Split(',').Select(e => e.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.Extension) && 
                                                extensions.Contains(e.TechnologyConfiguration.Extension));
            }

            if (!String.IsNullOrEmpty(imeiFilter))
            {
                var imeis = imeiFilter.Split(',').Select(i => i.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.IMEI) && 
                                                imeis.Contains(e.TechnologyConfiguration.IMEI));
            }

            if (!String.IsNullOrEmpty(simCardFilter))
            {
                var simCards = simCardFilter.Split(',').Select(s => s.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.SIMCardNumber) && 
                                                simCards.Contains(e.TechnologyConfiguration.SIMCardNumber));
            }

            if (!String.IsNullOrEmpty(configNotesFilter))
            {
                var configNotes = configNotesFilter.Split(',').Select(c => c.Trim()).ToList();
                equipment = equipment.Where(e => e.TechnologyConfiguration != null && 
                                                !String.IsNullOrEmpty(e.TechnologyConfiguration.ConfigurationNotes) && 
                                                configNotes.Contains(e.TechnologyConfiguration.ConfigurationNotes));
            }

            // Apply sorting
            equipment = sortOrder switch
            {
                "oath_tag_desc" => equipment.OrderByDescending(e => e.OATH_Tag),
                "netname" => equipment.OrderBy(e => e.TechnologyConfiguration != null ? e.TechnologyConfiguration.NetName : ""),
                "netname_desc" => equipment.OrderByDescending(e => e.TechnologyConfiguration != null ? e.TechnologyConfiguration.NetName : ""),
                "model" => equipment.OrderBy(e => e.Model),
                "model_desc" => equipment.OrderByDescending(e => e.Model),
                "status" => equipment.OrderBy(e => e.CurrentStatus != null ? e.CurrentStatus.Name : ""),
                "status_desc" => equipment.OrderByDescending(e => e.CurrentStatus != null ? e.CurrentStatus.Name : ""),
                "location" => equipment.OrderBy(e => e.CurrentLocation != null ? e.CurrentLocation.Name : ""),
                "location_desc" => equipment.OrderByDescending(e => e.CurrentLocation != null ? e.CurrentLocation.Name : ""),
                "floor" => equipment.OrderBy(e => e.CurrentFloorPlan != null ? e.CurrentFloorPlan.FloorName : ""),
                "floor_desc" => equipment.OrderByDescending(e => e.CurrentFloorPlan != null ? e.CurrentFloorPlan.FloorName : ""),
                "assigned" => equipment.OrderBy(e => e.Assigned_User_Name),
                "assigned_desc" => equipment.OrderByDescending(e => e.Assigned_User_Name),
                "category" => equipment.OrderBy(e => e.AssetCategory != null ? e.AssetCategory.Name : ""),
                "category_desc" => equipment.OrderByDescending(e => e.AssetCategory != null ? e.AssetCategory.Name : ""),
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

            var equipmentList = await equipment.AsNoTracking().ToListAsync();

            // Create Excel package
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Equipment");

                // Define all possible columns
                var allColumns = new Dictionary<string, string>
                {
                    { "oath-tag", "OATH Tag" },
                    { "assigned-to", "Assigned To" },
                    { "category", "Category" },
                    { "unit", "Unit" },
                    { "location", "Location" },
                    { "floor", "Floor" },
                    { "status", "Status" },
                    { "net-name", "Net Name" },
                    { "model", "Model" },
                    { "serial-number", "Serial Number" },
                    { "desk-number", "Desk Number" },
                    { "manufacturer", "Manufacturer" },
                    { "purchase-date", "Purchase Date" },
                    { "asset-tag", "Asset Tag" },
                    { "service-tag", "Service Tag" },
                    { "ip-address", "IP Address" },
                    { "os-version", "OS Version" },
                    { "phone-number", "Phone Number" },
                    { "purchase-price", "Purchase Price" },
                    { "warranty-end", "Warranty End" },
                    { "notes", "Notes" },
                    { "mac-address", "MAC Address" },
                    { "wall-port", "Wall Port" },
                    { "switch-name", "Switch Name" },
                    { "switch-port", "Switch Port" },
                    { "extension", "Extension" },
                    { "imei", "IMEI" },
                    { "sim-card", "SIM Card" },
                    { "config-notes", "Config Notes" },
                    { "created-at", "Created At" },
                    { "updated-at", "Updated At" }
                };

                // Determine visible columns
                var visibleColumnList = new List<string>();
                if (!string.IsNullOrEmpty(visibleColumns))
                {
                    visibleColumnList = visibleColumns.Split(',').Select(c => c.Trim()).ToList();
                }
                else
                {
                    // If no visible columns specified, show all
                    visibleColumnList = allColumns.Keys.ToList();
                }

                // Filter headers to only include visible columns
                var headers = visibleColumnList
                    .Where(col => allColumns.ContainsKey(col))
                    .Select(col => allColumns[col])
                    .ToArray();

                // Add headers
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    worksheet.Cells[1, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // Add data
                for (int row = 0; row < equipmentList.Count; row++)
                {
                    var item = equipmentList[row];
                    int col = 1;

                    foreach (var columnKey in visibleColumnList)
                    {
                        if (!allColumns.ContainsKey(columnKey)) continue;

                        switch (columnKey)
                        {
                            case "oath-tag":
                                worksheet.Cells[row + 2, col++].Value = item.OATH_Tag;
                                break;
                            case "assigned-to":
                                worksheet.Cells[row + 2, col++].Value = item.Assigned_User_Name;
                                break;
                            case "category":
                                worksheet.Cells[row + 2, col++].Value = item.AssetCategory?.Name;
                                break;
                            case "unit":
                                worksheet.Cells[row + 2, col++].Value = item.Department;
                                break;
                            case "location":
                                worksheet.Cells[row + 2, col++].Value = item.CurrentLocation?.Name;
                                break;
                            case "floor":
                                worksheet.Cells[row + 2, col++].Value = item.CurrentFloorPlan?.FloorName;
                                break;
                            case "status":
                                worksheet.Cells[row + 2, col++].Value = item.CurrentStatus?.Name;
                                break;
                            case "net-name":
                                worksheet.Cells[row + 2, col++].Value = item.TechnologyConfiguration?.NetName;
                                break;
                            case "model":
                                worksheet.Cells[row + 2, col++].Value = item.Model;
                                break;
                            case "serial-number":
                                worksheet.Cells[row + 2, col++].Value = item.Serial_Number;
                                break;
                            case "desk-number":
                                worksheet.Cells[row + 2, col++].Value = item.CurrentDesk?.DeskName;
                                break;
                            case "manufacturer":
                                worksheet.Cells[row + 2, col++].Value = item.Manufacturer;
                                break;
                            case "purchase-date":
                                worksheet.Cells[row + 2, col++].Value = item.PurchaseDate?.ToString("MM/dd/yyyy");
                                break;
                            case "asset-tag":
                                worksheet.Cells[row + 2, col++].Value = item.Asset_Tag;
                                break;
                            case "service-tag":
                                worksheet.Cells[row + 2, col++].Value = item.Service_Tag;
                                break;
                            case "ip-address":
                                worksheet.Cells[row + 2, col++].Value = item.IP_Address;
                                break;
                            case "os-version":
                                worksheet.Cells[row + 2, col++].Value = item.OS_Version;
                                break;
                            case "phone-number":
                                worksheet.Cells[row + 2, col++].Value = item.TechnologyConfiguration?.PhoneNumber;
                                break;
                            case "purchase-price":
                                worksheet.Cells[row + 2, col++].Value = item.PurchasePrice?.ToString("C");
                                break;
                            case "warranty-end":
                                worksheet.Cells[row + 2, col++].Value = item.WarrantyEndDate?.ToString("MM/dd/yyyy");
                                break;
                            case "notes":
                                worksheet.Cells[row + 2, col++].Value = item.Notes;
                                break;
                            case "mac-address":
                                worksheet.Cells[row + 2, col++].Value = item.TechnologyConfiguration?.MACAddress;
                                break;
                            case "wall-port":
                                worksheet.Cells[row + 2, col++].Value = item.TechnologyConfiguration?.WallPort;
                                break;
                            case "switch-name":
                                worksheet.Cells[row + 2, col++].Value = item.TechnologyConfiguration?.SwitchName;
                                break;
                            case "switch-port":
                                worksheet.Cells[row + 2, col++].Value = item.TechnologyConfiguration?.SwitchPort;
                                break;
                            case "extension":
                                worksheet.Cells[row + 2, col++].Value = item.TechnologyConfiguration?.Extension;
                                break;
                            case "imei":
                                worksheet.Cells[row + 2, col++].Value = item.TechnologyConfiguration?.IMEI;
                                break;
                            case "sim-card":
                                worksheet.Cells[row + 2, col++].Value = item.TechnologyConfiguration?.SIMCardNumber;
                                break;
                            case "config-notes":
                                worksheet.Cells[row + 2, col++].Value = item.TechnologyConfiguration?.ConfigurationNotes;
                                break;
                            case "created-at":
                                worksheet.Cells[row + 2, col++].Value = item.CreatedAt.ToString("MM/dd/yyyy HH:mm");
                                break;
                            case "updated-at":
                                worksheet.Cells[row + 2, col++].Value = item.UpdatedAt?.ToString("MM/dd/yyyy HH:mm");
                                break;
                        }
                    }
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Add borders to all cells
                var dataRange = worksheet.Cells[1, 1, equipmentList.Count + 1, headers.Length];
                dataRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                // Generate filename with timestamp
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var filename = $"Equipment_Export_{timestamp}.xlsx";

                // Return the Excel file
                var content = package.GetAsByteArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
        }

        // GET: Equipment/ExportSelected
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

        // Helper methods for select lists
        private async Task<SelectList> GetAssetCategorySelectList()
        {
            var categories = await _context.AssetCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return new SelectList(categories, "Id", "Name");
        }

        private async Task<SelectList> GetAssetStatusSelectList()
        {
            var statuses = await _context.AssetStatuses
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
            return new SelectList(statuses, "Id", "Name");
        }

        private async Task<SelectList> GetLocationSelectList()
        {
            var locations = await _context.Locations
                .Where(l => l.IsActive)
                .OrderBy(l => l.Name)
                .ToListAsync();
            return new SelectList(locations, "Id", "Name");
        }

        private async Task<SelectList> GetFloorPlanSelectList()
        {
            var floorPlans = await _context.FloorPlans
                .Where(fp => fp.IsActive)
                .OrderBy(fp => fp.FloorName)
                .ToListAsync();
            return new SelectList(floorPlans, "Id", "FloorName");
        }

        private async Task<SelectList> GetDeskSelectList()
        {
            var desks = await _context.Desks
                .Where(d => d.IsActive)
                .OrderBy(d => d.DeskName)
                .ToListAsync();
            return new SelectList(desks, "Id", "DeskName");
        }

        private async Task<SelectList> GetPersonSelectList()
        {
            var persons = await _context.People
                .Where(p => p.IsActive)
                .OrderBy(p => p.FullName)
                .ToListAsync();
            return new SelectList(persons, "Id", "FullName");
        }

        private async Task<SelectList> GetEntraUserSelectList()
        {
            var entraUsers = await _context.EntraUsers
                .Where(eu => eu.IsActive)
                .OrderBy(eu => eu.DisplayName)
                .ToListAsync();
            return new SelectList(entraUsers, "Id", "DisplayName");
        }

        private bool EquipmentExists(int id)
        {
            return _context.Equipment.Any(e => e.Id == id);
        }

        private async Task LogEquipmentAction(int equipmentId, string action, string description)
        {
            try
            {
                var auditLog = new AssetAuditLog
                {
                    EquipmentId = equipmentId,
                    Action = $"{action}: {description}",
                    PerformedBy = User.Identity?.Name ?? "System",
                    PerformedAt = DateTime.UtcNow
                };

                _context.AssetAuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid breaking the main operation
                Console.WriteLine($"Failed to log equipment action {action} for equipment {equipmentId}: {ex.Message}");
            }
        }

        private async Task LogEquipmentChanges(Equipment originalEquipment, Equipment updatedEquipment)
        {
            try
            {
                var changes = new List<string>();

                if (originalEquipment.OATH_Tag != updatedEquipment.OATH_Tag)
                    changes.Add($"OATH Tag: {originalEquipment.OATH_Tag} → {updatedEquipment.OATH_Tag}");

                if (originalEquipment.Serial_Number != updatedEquipment.Serial_Number)
                    changes.Add($"Serial Number: {originalEquipment.Serial_Number} → {updatedEquipment.Serial_Number}");

                if (originalEquipment.Model != updatedEquipment.Model)
                    changes.Add($"Model: {originalEquipment.Model} → {updatedEquipment.Model}");

                if (originalEquipment.Assigned_User_Name != updatedEquipment.Assigned_User_Name)
                    changes.Add($"Assigned User: {originalEquipment.Assigned_User_Name} → {updatedEquipment.Assigned_User_Name}");

                if (originalEquipment.CurrentStatusId != updatedEquipment.CurrentStatusId)
                    changes.Add($"Status: {originalEquipment.CurrentStatusId} → {updatedEquipment.CurrentStatusId}");

                if (originalEquipment.CurrentLocationId != updatedEquipment.CurrentLocationId)
                    changes.Add($"Location: {originalEquipment.CurrentLocationId} → {updatedEquipment.CurrentLocationId}");

                if (changes.Any())
                {
                    await LogEquipmentAction(updatedEquipment.Id, "Updated", $"Changes: {string.Join(", ", changes)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log equipment changes: {ex.Message}");
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
