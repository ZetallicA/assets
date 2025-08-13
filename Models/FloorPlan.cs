using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Models
{
    public class FloorPlan
    {
        public int Id { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Floor Number")]
        public string FloorNumber { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Floor Name")]
        public string? FloorName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        [Display(Name = "Image Path")]
        public string? ImagePath { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("LocationId")]
        public virtual Location? Location { get; set; }

        public virtual ICollection<Desk> Desks { get; set; } = new List<Desk>();
        public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
    }
}
