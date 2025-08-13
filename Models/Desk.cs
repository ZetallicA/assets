using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    public class Desk
    {
        public int Id { get; set; }

        [Required]
        public int FloorPlanId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Desk Number")]
        public string DeskNumber { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Desk Name")]
        public string? DeskName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int XCoordinate { get; set; }
        public int YCoordinate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("FloorPlanId")]
        public virtual FloorPlan? FloorPlan { get; set; }

        public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
    }
}
