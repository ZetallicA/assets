using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    public class SimplifiedEquipment
    {
        [Key]
        public int Id { get; set; }

        // Primary Identifiers
        [Required]
        [StringLength(100)]
        public string Asset_Tag { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Serial_Number { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Service_Tag { get; set; }

        // Basic Equipment Info
        [StringLength(100)]
        public string? Manufacturer { get; set; }

        [StringLength(100)]
        public string? Model { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(100)]
        public string? Net_Name { get; set; }

        // Assignment Info
        [StringLength(100)]
        public string? Assigned_User_Name { get; set; }

        [StringLength(100)]
        public string? Assigned_User_Email { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? Unit { get; set; }

        // Location Info
        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(100)]
        public string? Floor { get; set; }

        [StringLength(100)]
        public string? Desk { get; set; }

        // Status
        [StringLength(50)]
        public string? Status { get; set; }

        // Network Info

        [StringLength(15)]
        public string? IP_Address { get; set; }

        [StringLength(17)]
        public string? MAC_Address { get; set; }

        [StringLength(100)]
        public string? Wall_Port { get; set; }

        [StringLength(100)]
        public string? Switch_Name { get; set; }

        [StringLength(50)]
        public string? Switch_Port { get; set; }

        // Phone Info
        [StringLength(20)]
        public string? Phone_Number { get; set; }

        [StringLength(20)]
        public string? Extension { get; set; }

        // Mobile Device Info
        [StringLength(50)]
        public string? IMEI { get; set; }

        [StringLength(50)]
        public string? SIM_Card_Number { get; set; }

        // OS and Software
        [StringLength(100)]
        public string? OS_Version { get; set; }

        [StringLength(100)]
        public string? License1 { get; set; }

        [StringLength(100)]
        public string? License2 { get; set; }

        [StringLength(100)]
        public string? License3 { get; set; }

        [StringLength(100)]
        public string? License4 { get; set; }

        [StringLength(100)]
        public string? License5 { get; set; }

        // Financial Info
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Purchase_Price { get; set; }

        [StringLength(100)]
        public string? Purchase_Order_Number { get; set; }

        [StringLength(100)]
        public string? Vendor { get; set; }

        [StringLength(500)]
        public string? Vendor_Info { get; set; }

        public DateTime? Purchase_Date { get; set; }

        public DateTime? Warranty_Start_Date { get; set; }

        public DateTime? Warranty_End_Date { get; set; }

        // Additional Info
        [StringLength(500)]
        public string? Notes { get; set; }

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; } = "System";

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        // Checkout Info
        public bool IsCheckedOut { get; set; } = false;

        public DateTime? CheckedOutDate { get; set; }

        public string? CheckedOutBy { get; set; }

        public DateTime? ExpectedReturnDate { get; set; }

        // QR Code and Barcode
        [StringLength(500)]
        public string? QRCode { get; set; }

        [StringLength(100)]
        public string? Barcode { get; set; }
    }
}
