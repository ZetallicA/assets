using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Models
{
    public class EntraUser
    {
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string ObjectId { get; set; } = string.Empty;

        [StringLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [StringLength(100)]
        public string UserPrincipalName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Mail { get; set; }

        [StringLength(50)]
        public string? GivenName { get; set; }

        [StringLength(50)]
        public string? Surname { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? OfficeLocation { get; set; }

        [StringLength(50)]
        public string? EmployeeId { get; set; }

        [StringLength(50)]
        public string? EmployeeType { get; set; }

        public bool AccountEnabled { get; set; } = true;

        public DateTime? LastSignInDateTime { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? LastSyncDateTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Equipment> AssignedEquipment { get; set; } = new List<Equipment>();
    }
}
