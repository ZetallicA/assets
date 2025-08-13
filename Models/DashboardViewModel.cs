using AssetManagement.Models;

namespace AssetManagement.Models
{
    public class DashboardViewModel
    {
        public int TotalEquipment { get; set; }
        public int AssignedEquipment { get; set; }
        public int UnassignedEquipment { get; set; }
        public int TotalLocations { get; set; }
        public int TotalFloorPlans { get; set; }
        public int TotalDesks { get; set; }
        public int TotalUsers { get; set; }
        public List<Equipment> RecentEquipment { get; set; } = new List<Equipment>();
        public List<Equipment> UnassignedEquipmentList { get; set; } = new List<Equipment>();
        public List<Location> RecentLocations { get; set; } = new List<Location>();
    }
}
