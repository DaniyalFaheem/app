using Microsoft.EntityFrameworkCore;
using FaceRecognitionAttendance.Models;
using System;

namespace FaceRecognitionAttendance.Data
{
    /// <summary>
    /// Entity Framework Core database context
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null! ;
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; } = null!;
        public DbSet<AdminUser> AdminUsers { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhotoPath).HasMaxLength(500);
                entity.Property(e => e.ModifiedBy).HasMaxLength(100);
                entity.Property(e => e.FaceEncoding).IsRequired();
                
                entity.HasIndex(e => e.Phone);
                entity.HasIndex(e => e.Department);
                entity.HasIndex(e => e.UserType);
            });

            // AttendanceRecord entity configuration
            modelBuilder.Entity<AttendanceRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.AttendanceRecords)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.DateTime);
                entity.HasIndex(e => e.Department);
            });

            // AdminUser entity configuration
            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity. Property(e => e.Username). IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                
                entity.HasIndex(e => e.Username). IsUnique();
            });

            // AuditLog entity configuration
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e. Action).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Details).HasMaxLength(2000);
                
                entity.HasOne(e => e.AdminUser)
                    .WithMany(u => u. AuditLogs)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasIndex(e => e. Timestamp);
            });
        }
    }
}