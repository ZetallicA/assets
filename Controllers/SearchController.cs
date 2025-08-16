using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;

namespace AssetManagement.Controllers
{
    [Route("[controller]")]
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Suggest")]
        public async Task<IActionResult> Suggest([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            {
                return Json(new List<object>());
            }

            var query = q.ToLower();
            var results = new List<object>();

            // Search Equipment
            var equipmentResults = await _context.Equipment
                .Include(e => e.TechnologyConfiguration)
                .Where(e => e.IsActive && (
                    e.OATH_Tag.ToLower().Contains(query) ||
                    (e.Serial_Number != null && e.Serial_Number.ToLower().Contains(query)) ||
                    (e.Model != null && e.Model.ToLower().Contains(query)) ||
                    (e.Department != null && e.Department.ToLower().Contains(query)) ||
                    (e.TechnologyConfiguration != null && 
                     ((e.TechnologyConfiguration.NetName != null && e.TechnologyConfiguration.NetName.ToLower().Contains(query)) ||
                      (e.TechnologyConfiguration.IPv4Address != null && e.TechnologyConfiguration.IPv4Address.ToLower().Contains(query))))
                ))
                .Take(5)
                .Select(e => new
                {
                    type = "equipment",
                    id = e.Id,
                    label = $"{e.OATH_Tag} - {e.Model ?? "Unknown Model"}",
                    meta = $"{e.Department ?? "No Department"} • {e.CurrentStatusId}",
                    actionUrls = new
                    {
                        view = Url.Action("Details", "Equipment", new { id = e.Id }),
                        edit = Url.Action("Edit", "Equipment", new { id = e.Id })
                    }
                })
                .ToListAsync();

            results.AddRange(equipmentResults);

            // Search Technology Configurations
            var techConfigResults = await _context.TechnologyConfigurations
                .Where(tc => (
                    (tc.NetName != null && tc.NetName.ToLower().Contains(query)) ||
                    (tc.IPv4Address != null && tc.IPv4Address.ToLower().Contains(query)) ||
                    (tc.MACAddress != null && tc.MACAddress.ToLower().Contains(query)) ||
                    (tc.WallPort != null && tc.WallPort.ToLower().Contains(query))
                ))
                .Take(3)
                .Select(tc => new
                {
                    type = "techconfig",
                    id = tc.Id,
                    label = $"{tc.NetName ?? "Unknown"} ({tc.IPv4Address ?? "No IP"})",
                    meta = $"Tech Config • {tc.WallPort ?? "No Port"}",
                    actionUrls = new
                    {
                        view = Url.Action("Details", "TechnologyConfiguration", new { id = tc.Id }),
                        edit = Url.Action("Edit", "TechnologyConfiguration", new { id = tc.Id })
                    }
                })
                .ToListAsync();

            results.AddRange(techConfigResults);

            // Search Floor Plans
            var floorPlanResults = await _context.FloorPlans
                .Include(fp => fp.Location)
                .Where(fp => fp.IsActive && (
                    fp.Location.Name.ToLower().Contains(query) ||
                    fp.FloorName.ToLower().Contains(query) ||
                    fp.FloorNumber.ToString().Contains(query)
                ))
                .Take(3)
                .Select(fp => new
                {
                    type = "floorplan",
                    id = fp.Id,
                    label = $"{fp.Location.Name} - Floor {fp.FloorNumber}",
                    meta = fp.FloorName ?? "Floor Plan",
                    actionUrls = new
                    {
                        view = Url.Action("FloorPlan", "InteractiveMap", new { id = fp.Id }),
                        edit = Url.Action("Edit", "FloorPlan", new { id = fp.Id })
                    }
                })
                .ToListAsync();

            results.AddRange(floorPlanResults);

            // Search Entra Users
            var userResults = await _context.EntraUsers
                .Where(u => u.IsActive && (
                    u.DisplayName.ToLower().Contains(query) ||
                    (u.Mail != null && u.Mail.ToLower().Contains(query)) ||
                    (u.Department != null && u.Department.ToLower().Contains(query))
                ))
                .Take(3)
                .Select(u => new
                {
                    type = "user",
                    id = u.Id,
                    label = u.DisplayName,
                    meta = $"{u.Mail ?? "No Email"} • {u.Department ?? "No Department"}",
                    actionUrls = new
                    {
                        view = Url.Action("Details", "EntraUser", new { id = u.Id }),
                        edit = Url.Action("Edit", "EntraUser", new { id = u.Id })
                    }
                })
                .ToListAsync();

            results.AddRange(userResults);

            return Json(results);
        }

        [HttpGet("Global")]
        public async Task<IActionResult> Global([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return View(new GlobalSearchViewModel { Query = q, Results = new List<SearchResult>() });
            }

            var query = q.ToLower();
            var results = new List<SearchResult>();

            // Equipment search
            var equipment = await _context.Equipment
                .Include(e => e.TechnologyConfiguration)
                .Include(e => e.AssignedEntraUser)
                .Where(e => e.IsActive && (
                    e.OATH_Tag.ToLower().Contains(query) ||
                    (e.Serial_Number != null && e.Serial_Number.ToLower().Contains(query)) ||
                    (e.Model != null && e.Model.ToLower().Contains(query)) ||
                    (e.Department != null && e.Department.ToLower().Contains(query)) ||
                    (e.TechnologyConfiguration != null && 
                     ((e.TechnologyConfiguration.NetName != null && e.TechnologyConfiguration.NetName.ToLower().Contains(query)) ||
                      (e.TechnologyConfiguration.IPv4Address != null && e.TechnologyConfiguration.IPv4Address.ToLower().Contains(query))))
                ))
                .Take(20)
                .ToListAsync();

            foreach (var e in equipment)
            {
                results.Add(new SearchResult
                {
                    Type = "Equipment",
                    Id = e.Id,
                    Title = $"{e.OATH_Tag} - {e.Model ?? "Unknown Model"}",
                    Subtitle = $"{e.Department ?? "No Department"} • {e.CurrentStatusId}",
                    Description = e.TechnologyConfiguration?.NetName ?? "No network name",
                    Url = Url.Action("Details", "Equipment", new { id = e.Id }),
                    Icon = "fas fa-desktop"
                });
            }

            // Technology Configurations
            var techConfigs = await _context.TechnologyConfigurations
                .Where(tc => (
                    (tc.NetName != null && tc.NetName.ToLower().Contains(query)) ||
                    (tc.IPv4Address != null && tc.IPv4Address.ToLower().Contains(query)) ||
                    (tc.MACAddress != null && tc.MACAddress.ToLower().Contains(query))
                ))
                .Take(10)
                .ToListAsync();

            foreach (var tc in techConfigs)
            {
                results.Add(new SearchResult
                {
                    Type = "Technology Configuration",
                    Id = tc.Id,
                    Title = tc.NetName ?? "Unknown",
                    Subtitle = $"IP: {tc.IPv4Address ?? "No IP"} • Port: {tc.WallPort ?? "No Port"}",
                    Description = $"MAC: {tc.MACAddress ?? "No MAC"}",
                    Url = Url.Action("Details", "TechnologyConfiguration", new { id = tc.Id }),
                    Icon = "fas fa-network-wired"
                });
            }

            // Floor Plans
            var floorPlans = await _context.FloorPlans
                .Include(fp => fp.Location)
                .Where(fp => fp.IsActive && (
                    fp.Location.Name.ToLower().Contains(query) ||
                    fp.FloorName.ToLower().Contains(query)
                ))
                .Take(10)
                .ToListAsync();

            foreach (var fp in floorPlans)
            {
                results.Add(new SearchResult
                {
                    Type = "Floor Plan",
                    Id = fp.Id,
                    Title = $"{fp.Location.Name} - Floor {fp.FloorNumber}",
                    Subtitle = fp.FloorName ?? "Floor Plan",
                    Description = $"{fp.Desks.Count(d => d.IsActive)} desks",
                    Url = Url.Action("FloorPlan", "InteractiveMap", new { id = fp.Id }),
                    Icon = "fas fa-map"
                });
            }

            var viewModel = new GlobalSearchViewModel
            {
                Query = q,
                Results = results.OrderBy(r => r.Type).ThenBy(r => r.Title).ToList()
            };

            return View(viewModel);
        }
    }

    public class GlobalSearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public List<SearchResult> Results { get; set; } = new();
    }

    public class SearchResult
    {
        public string Type { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
