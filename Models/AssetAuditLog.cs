using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    public class AssetAuditLog
    {
        public int Id { get; set; }

        [Required]
        public int EquipmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [StringLength(100)]
        public string? PerformedBy { get; set; }

        [StringLength(100)]
        public string? PerformedByEmail { get; set; }

        [StringLength(50)]
        public string? PreviousStatus { get; set; }

        [StringLength(50)]
        public string? NewStatus { get; set; }

        [StringLength(100)]
        public string? PreviousLocation { get; set; }

        [StringLength(100)]
        public string? NewLocation { get; set; }

        [StringLength(100)]
        public string? PreviousAssignee { get; set; }

        [StringLength(100)]
        public string? NewAssignee { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [StringLength(1000)]
        public string? AdditionalData { get; set; }

        [StringLength(500)]
        public string? Details { get; set; }

        [StringLength(100)]
        public string? UserId { get; set; }

        // Navigation properties
        [ForeignKey("EquipmentId")]
        public virtual Equipment? Equipment { get; set; }
    }
}
