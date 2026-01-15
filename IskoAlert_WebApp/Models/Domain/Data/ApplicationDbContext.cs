using IskoAlert_WebApp.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace IskoAlert_WebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- Core Tables based on SRS Documentation ---
        public DbSet<User> Users { get; set; }
        public DbSet<IncidentReport> IncidentReports { get; set; }
        //public DbSet<LostFoundItem> LostFoundItems { get; set; }
        //public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. USER CONFIGURATION
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.HasIndex(u => u.IdNumber).IsUnique();
                entity.HasIndex(u => u.Webmail).IsUnique();

                entity.Property(u => u.IdNumber).IsRequired().HasMaxLength(20);
                entity.Property(u => u.Webmail).IsRequired().HasMaxLength(100);

                // Enums: Roles (Student, Faculty, Admin)
                entity.Property(u => u.Role).HasConversion<string>().IsRequired();
                entity.Property(u => u.AccountStatus).HasConversion<string>().IsRequired();
            });

            // 2. INCIDENT REPORT CONFIGURATION
            modelBuilder.Entity<IncidentReport>(entity =>
            {
                entity.HasKey(ir => ir.ReportId);

                // Added Campus Location as requested
                entity.Property(ir => ir.CampusLocation).IsRequired().HasMaxLength(150);

                entity.Property(ir => ir.Title).IsRequired().HasMaxLength(150);
                entity.Property(ir => ir.Description).IsRequired().HasMaxLength(500); // SRS Requirement
                entity.Property(ir => ir.ImagePath).HasMaxLength(255); // For JPG/PNG evidence

                // Enums: Status (Pending, Accepted, In-Progress, Resolved, Rejected)
                entity.Property(ir => ir.Status).HasConversion<string>().IsRequired();

                entity.HasOne(ir => ir.User)
                      .WithMany()
                      .HasForeignKey(ir => ir.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            //// 3. LOST AND FOUND CONFIGURATION
            //modelBuilder.Entity<LostFoundItem>(entity =>
            //{
            //    entity.HasKey(lfi => lfi.ItemId);
            //    entity.Property(lfi => lfi.ItemName).IsRequired().HasMaxLength(100);
            //    entity.Property(lfi => lfi.Description).HasMaxLength(500);
            //    entity.Property(lfi => lfi.LocationFound).IsRequired().HasMaxLength(150);
            //    entity.Property(lfi => lfi.ItemImagePath).HasMaxLength(255);

            //    // Enums: Status (Lost, Found, Claimed), Category
            //    entity.Property(lfi => lfi.Status).HasConversion<string>().IsRequired();
            //    entity.Property(lfi => lfi.Category).HasConversion<string>().IsRequired();

            //    entity.HasOne(lfi => lfi.User)
            //          .WithMany()
            //          .HasForeignKey(lfi => lfi.UserId)
            //          .OnDelete(DeleteBehavior.Cascade);
            //});

            //// 4. NOTIFICATION CONFIGURATION
            //modelBuilder.Entity<Notification>(entity =>
            //{
            //    entity.HasKey(n => n.NotificationId);
            //    entity.Property(n => n.Message).IsRequired().HasMaxLength(255);
            //    entity.Property(n => n.CreatedAt).IsRequired();

            //    // Enum: Notification Type
            //    entity.Property(n => n.Type).HasConversion<string>().IsRequired();

            //    entity.HasOne(n => n.User)
            //          .WithMany()
            //          .HasForeignKey(n => n.UserId)
            //          .OnDelete(DeleteBehavior.Cascade);
            //});
        }
    }
}