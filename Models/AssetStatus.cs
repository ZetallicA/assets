using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Models
{
    public class AssetStatus
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        [StringLength(7)]
        public string? Color { get; set; } = "#007bff";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
    }
}
