using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    public class TechnologyConfiguration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Equipment")]
        public int EquipmentId { get; set; }

        [Display(Name = "Net Name")]
        [StringLength(100)]
        public string? NetName { get; set; }

        [Display(Name = "IPv4 Address")]
        [StringLength(15)]
        public string? IPv4Address { get; set; }

        [Display(Name = "MAC Address")]
        [StringLength(17)]
        public string? MACAddress { get; set; }

        [Display(Name = "Wall Port")]
        [StringLength(50)]
        public string? WallPort { get; set; }

        [Display(Name = "Switch Port")]
        [StringLength(50)]
        public string? SwitchPort { get; set; }

        [Display(Name = "Switch Name")]
        [StringLength(100)]
        public string? SwitchName { get; set; }

        [Display(Name = "Phone Number")]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Extension")]
        [StringLength(10)]
        public string? Extension { get; set; }

        [Display(Name = "IMEI")]
        [StringLength(20)]
        public string? IMEI { get; set; }

        [Display(Name = "SIM Card Number")]
        [StringLength(25)]
        public string? SIMCardNumber { get; set; }

        [Display(Name = "Vendor/ISP")]
        [StringLength(100)]
        public string? Vendor { get; set; }

        // License Information
        [Display(Name = "License 1")]
        [StringLength(200)]
        public string? License1 { get; set; }

        [Display(Name = "License 2")]
        [StringLength(200)]
        public string? License2 { get; set; }

        [Display(Name = "License 3")]
        [StringLength(200)]
        public string? License3 { get; set; }

        [Display(Name = "License 4")]
        [StringLength(200)]
        public string? License4 { get; set; }

        [Display(Name = "License 5")]
        [StringLength(200)]
        public string? License5 { get; set; }

        [Display(Name = "Configuration Notes")]
        [StringLength(1000)]
        public string? ConfigurationNotes { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime? LastUpdated { get; set; }

        [Display(Name = "Updated By")]
        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        // Navigation property
        public virtual Equipment Equipment { get; set; } = null!;
    }
}
