using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Models;
using AssetManagement.Data;

namespace AssetManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var dashboard = new DashboardViewModel
        {
            TotalEquipment = await _context.Equipment.Where(e => e.IsActive).CountAsync(),
            AssignedEquipment = await _context.Equipment.Where(e => e.IsActive && e.AssignedEntraUserId != null).CountAsync(),
            UnassignedEquipment = await _context.Equipment.Where(e => e.IsActive && e.AssignedEntraUserId == null).CountAsync(),
            TotalLocations = await _context.Locations.Where(l => l.IsActive).CountAsync(),
            TotalFloorPlans = await _context.FloorPlans.Where(f => f.IsActive).CountAsync(),
            TotalDesks = await _context.Desks.Where(d => d.IsActive).CountAsync(),
            TotalUsers = await _context.EntraUsers.Where(u => u.IsActive).CountAsync(),
                            RecentEquipment = await _context.Equipment
                    .Include(e => e.AssetCategory)
                    .Include(e => e.CurrentStatus)
                    .Include(e => e.AssignedEntraUser)
                    .Where(e => e.IsActive)
                    .OrderByDescending(e => e.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                UnassignedEquipmentList = await _context.Equipment
                    .Include(e => e.AssetCategory)
                    .Include(e => e.CurrentStatus)
                    .Where(e => e.IsActive && e.AssignedEntraUserId == null)
                    .OrderByDescending(e => e.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
            RecentLocations = await _context.Locations
                .Where(l => l.IsActive)
                .OrderByDescending(l => l.CreatedAt)
                .Take(3)
                .ToListAsync()
        };

        return View(dashboard);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
