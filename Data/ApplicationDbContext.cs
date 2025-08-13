using Microsoft.EntityFrameworkCore;
using AssetManagement.Models;

namespace AssetManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<AssetCategory> AssetCategories { get; set; }
        public DbSet<AssetStatus> AssetStatuses { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<FloorPlan> FloorPlans { get; set; }
        public DbSet<Desk> Desks { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<EntraUser> EntraUsers { get; set; }
        public DbSet<AssetAuditLog> AssetAuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Equipment configuration
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OATH_Tag).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.OATH_Tag).IsUnique();
                entity.Property(e => e.Serial_Number).HasMaxLength(100);
                entity.Property(e => e.Manufacturer).HasMaxLength(100);
                entity.Property(e => e.Model).HasMaxLength(100);
                entity.Property(e => e.Computer_Name).HasMaxLength(100);
                entity.Property(e => e.PurchaseOrderNumber).HasMaxLength(50);
                entity.Property(e => e.CostCentre).HasMaxLength(100);
                entity.Property(e => e.Condition).HasMaxLength(50);
                entity.Property(e => e.Barcode).HasMaxLength(100);
                entity.Property(e => e.QRCode).HasMaxLength(100);
                entity.Property(e => e.Assigned_User_Name).HasMaxLength(100);
                entity.Property(e => e.Assigned_User_Email).HasMaxLength(100);
                entity.Property(e => e.Assigned_User_ID).HasMaxLength(50);
                entity.Property(e => e.AssignedEntraObjectId).HasMaxLength(450);
                entity.Property(e => e.Phone_Number).HasMaxLength(20);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Facility).HasMaxLength(100);
                entity.Property(e => e.OS_Version).HasMaxLength(50);
                entity.Property(e => e.IP_Address).HasMaxLength(15);
                entity.Property(e => e.CheckedOutByUserId).HasMaxLength(100);
                entity.Property(e => e.Current_Location_Notes).HasMaxLength(500);

                // Relationships
                entity.HasOne(e => e.AssetCategory)
                    .WithMany(ac => ac.Equipment)
                    .HasForeignKey(e => e.AssetCategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.CurrentStatus)
                    .WithMany(s => s.Equipment)
                    .HasForeignKey(e => e.CurrentStatusId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.CurrentLocation)
                    .WithMany(l => l.Equipment)
                    .HasForeignKey(e => e.CurrentLocationId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.CurrentFloorPlan)
                    .WithMany(fp => fp.Equipment)
                    .HasForeignKey(e => e.CurrentFloorPlanId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.CurrentDesk)
                    .WithMany(d => d.Equipment)
                    .HasForeignKey(e => e.CurrentDeskId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.AssignedPerson)
                    .WithMany(p => p.AssignedEquipment)
                    .HasForeignKey(e => e.AssignedPersonId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.AssignedEntraUser)
                    .WithMany(eu => eu.AssignedEquipment)
                    .HasForeignKey(e => e.AssignedEntraUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // AssetCategory configuration
            modelBuilder.Entity<AssetCategory>(entity =>
            {
                entity.HasKey(ac => ac.Id);
                entity.Property(ac => ac.Name).IsRequired().HasMaxLength(100);
                entity.Property(ac => ac.Description).HasMaxLength(500);
                entity.HasIndex(ac => ac.Name).IsUnique();
            });

            // AssetStatus configuration
            modelBuilder.Entity<AssetStatus>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(50);
                entity.Property(s => s.Description).HasMaxLength(200);
                entity.Property(s => s.Color).HasMaxLength(7);
                entity.HasIndex(s => s.Name).IsUnique();
            });

            // Location configuration
            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Name).IsRequired().HasMaxLength(100);
                entity.Property(l => l.Address).HasMaxLength(200);
                entity.Property(l => l.City).HasMaxLength(100);
                entity.Property(l => l.State).HasMaxLength(50);
                entity.Property(l => l.ZipCode).HasMaxLength(20);
                entity.Property(l => l.Notes).HasMaxLength(500);
            });

            // FloorPlan configuration
            modelBuilder.Entity<FloorPlan>(entity =>
            {
                entity.HasKey(fp => fp.Id);
                entity.Property(fp => fp.FloorNumber).IsRequired().HasMaxLength(50);
                entity.Property(fp => fp.FloorName).HasMaxLength(100);
                entity.Property(fp => fp.Description).HasMaxLength(500);
                entity.Property(fp => fp.ImagePath).HasMaxLength(500);

                entity.HasOne(fp => fp.Location)
                    .WithMany(l => l.FloorPlans)
                    .HasForeignKey(fp => fp.LocationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Desk configuration
            modelBuilder.Entity<Desk>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.DeskNumber).IsRequired().HasMaxLength(50);
                entity.Property(d => d.DeskName).HasMaxLength(100);
                entity.Property(d => d.Description).HasMaxLength(500);

                entity.HasOne(d => d.FloorPlan)
                    .WithMany(fp => fp.Desks)
                    .HasForeignKey(d => d.FloorPlanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Person configuration
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.FullName).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Email).HasMaxLength(100);
                entity.Property(p => p.Phone).HasMaxLength(20);
                entity.Property(p => p.Supervisor).HasMaxLength(100);
            });

            // EntraUser configuration
            modelBuilder.Entity<EntraUser>(entity =>
            {
                entity.HasKey(eu => eu.Id);
                entity.Property(eu => eu.ObjectId).IsRequired().HasMaxLength(450);
                entity.Property(eu => eu.DisplayName).IsRequired().HasMaxLength(100);
                entity.Property(eu => eu.UserPrincipalName).IsRequired().HasMaxLength(100);
                entity.Property(eu => eu.Mail).HasMaxLength(100);
                entity.Property(eu => eu.GivenName).HasMaxLength(50);
                entity.Property(eu => eu.Surname).HasMaxLength(50);
                entity.Property(eu => eu.JobTitle).HasMaxLength(100);
                entity.Property(eu => eu.Department).HasMaxLength(100);
                entity.Property(eu => eu.OfficeLocation).HasMaxLength(100);
                entity.Property(eu => eu.EmployeeId).HasMaxLength(50);
                entity.Property(eu => eu.EmployeeType).HasMaxLength(50);

                entity.HasIndex(eu => eu.ObjectId).IsUnique();
                entity.HasIndex(eu => eu.UserPrincipalName).IsUnique();
            });

            // AssetAuditLog configuration
            modelBuilder.Entity<AssetAuditLog>(entity =>
            {
                entity.HasKey(aal => aal.Id);
                entity.Property(aal => aal.Action).IsRequired().HasMaxLength(100);
                entity.Property(aal => aal.PerformedBy).HasMaxLength(100);
                entity.Property(aal => aal.PerformedByEmail).HasMaxLength(100);
                entity.Property(aal => aal.PreviousStatus).HasMaxLength(50);
                entity.Property(aal => aal.NewStatus).HasMaxLength(50);
                entity.Property(aal => aal.PreviousLocation).HasMaxLength(100);
                entity.Property(aal => aal.NewLocation).HasMaxLength(100);
                entity.Property(aal => aal.PreviousAssignee).HasMaxLength(100);
                entity.Property(aal => aal.NewAssignee).HasMaxLength(100);
                entity.Property(aal => aal.IpAddress).HasMaxLength(45);
                entity.Property(aal => aal.UserAgent).HasMaxLength(500);
                entity.Property(aal => aal.AdditionalData).HasMaxLength(1000);

                entity.HasOne(aal => aal.Equipment)
                    .WithMany(e => e.AuditLogs)
                    .HasForeignKey(aal => aal.EquipmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
