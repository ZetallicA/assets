using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    public class Equipment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "OATH Tag")]
        public string OATH_Tag { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Serial Number")]
        public string? Serial_Number { get; set; }

        [StringLength(100)]
        [Display(Name = "Asset Tag")]
        public string? Asset_Tag { get; set; }

        [StringLength(50)]
        [Display(Name = "Service Tag")]
        public string? Service_Tag { get; set; }

        public int? AssetCategoryId { get; set; }

        [NotMapped]
        public int? CategoryId => AssetCategoryId;

        [NotMapped]
        public int? StatusId => CurrentStatusId;

        [StringLength(100)]
        public string? Manufacturer { get; set; }

        [StringLength(100)]
        public string? Model { get; set; }

        [StringLength(100)]
        [Display(Name = "Computer Name")]
        public string? Computer_Name { get; set; }

        [StringLength(50)]
        [Display(Name = "Purchase Order Number")]
        public string? PurchaseOrderNumber { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Purchase Price")]
        [DataType(DataType.Currency)]
        public decimal? PurchasePrice { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Purchase Cost")]
        [DataType(DataType.Currency)]
        public decimal? Purchase_Cost { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Current Value")]
        [DataType(DataType.Currency)]
        public decimal? Current_Value { get; set; }

        [StringLength(100)]
        [Display(Name = "Cost Centre")]
        public string? CostCentre { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Purchase Date")]
        public DateTime? PurchaseDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Purchase Date")]
        public DateTime? Purchase_Date { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Warranty Start Date")]
        public DateTime? WarrantyStartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Warranty End Date")]
        public DateTime? WarrantyEndDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Warranty Expiry")]
        public DateTime? Warranty_Expiry { get; set; }

        [StringLength(50)]
        [Display(Name = "Condition")]
        public string Condition { get; set; } = "Good";

        [StringLength(100)]
        [Display(Name = "Barcode")]
        public string? Barcode { get; set; }

        [StringLength(100)]
        [Display(Name = "QR Code")]
        public string? QRCode { get; set; }

        [StringLength(100)]
        [Display(Name = "Assigned User's Name")]
        public string? Assigned_User_Name { get; set; }

        [StringLength(100)]
        [Display(Name = "Assigned User Email")]
        public string? Assigned_User_Email { get; set; }

        [StringLength(50)]
        [Display(Name = "Assigned User ID")]
        public string? Assigned_User_ID { get; set; }

        public int? AssignedPersonId { get; set; }

        public int? AssignedEntraUserId { get; set; }

        [StringLength(450)]
        public string? AssignedEntraObjectId { get; set; }

        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string? Phone_Number { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Expected Return Date")]
        public DateTime? ExpectedReturnDate { get; set; }

        public int? CurrentStatusId { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(100)]
        [Display(Name = "Facility")]
        public string? Facility { get; set; }

        [StringLength(50)]
        [Display(Name = "OS Version")]
        public string? OS_Version { get; set; }

        [StringLength(15)]
        [Display(Name = "IP Address")]
        public string? IP_Address { get; set; }

        [Display(Name = "Checked Out")]
        public bool IsCheckedOut { get; set; } = false;

        [StringLength(100)]
        [Display(Name = "Checked Out By User ID")]
        public string? CheckedOutByUserId { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Checked Out Date")]
        public DateTime? CheckedOutDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Check-in Date")]
        public DateTime? LastCheckInDate { get; set; }

        [Display(Name = "Current Location")]
        public int? CurrentLocationId { get; set; }

        [Display(Name = "Current Floor Plan")]
        public int? CurrentFloorPlanId { get; set; }

        [Display(Name = "Current Desk")]
        public int? CurrentDeskId { get; set; }

        [StringLength(500)]
        [Display(Name = "Current Location Notes")]
        public string? Current_Location_Notes { get; set; }

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Current Book Value")]
        [DataType(DataType.Currency)]
        public decimal? CurrentBookValue { get; set; }

        [Display(Name = "Depreciation Rate")]
        public decimal? DepreciationRate { get; set; }

        [Display(Name = "Last Depreciation Calculation")]
        public DateTime? LastDepreciationCalculation { get; set; }

        [Display(Name = "Last Maintenance Date")]
        public DateTime? LastMaintenanceDate { get; set; }

        [Display(Name = "Last Maintenance Date")]
        public DateTime? Last_Maintenance_Date { get; set; }

        [Display(Name = "Next Maintenance Date")]
        public DateTime? NextMaintenanceDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("AssetCategoryId")]
        public virtual AssetCategory? AssetCategory { get; set; }

        [ForeignKey("CurrentStatusId")]
        public virtual AssetStatus? CurrentStatus { get; set; }

        [ForeignKey("CurrentLocationId")]
        public virtual Location? CurrentLocation { get; set; }

        [ForeignKey("CurrentFloorPlanId")]
        public virtual FloorPlan? CurrentFloorPlan { get; set; }

        [ForeignKey("CurrentDeskId")]
        public virtual Desk? CurrentDesk { get; set; }

        [ForeignKey("AssignedPersonId")]
        public virtual Person? AssignedPerson { get; set; }

        [ForeignKey("AssignedEntraUserId")]
        public virtual EntraUser? AssignedEntraUser { get; set; }

        public virtual ICollection<AssetAuditLog> AuditLogs { get; set; } = new List<AssetAuditLog>();

        // NotMapped properties for simplified access
        [NotMapped]
        public string? Building => CurrentLocation?.Name;

        [NotMapped]
        public string? Floor => CurrentFloorPlan?.FloorNumber?.ToString();

        [NotMapped]
        public string? Room => CurrentDesk?.DeskNumber;

        [NotMapped]
        public string? StatusName => CurrentStatus?.Name ?? "Unknown";

        [NotMapped]
        public string? CategoryName => AssetCategory?.Name ?? "Uncategorized";
    }
}
