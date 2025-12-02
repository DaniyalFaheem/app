using Microsoft. EntityFrameworkCore;
using FaceRecognitionAttendance.Models;
using System;
using System. Linq;
using System.Threading. Tasks;

namespace FaceRecognitionAttendance.Data
{
    /// <summary>
    /// Database initialization and seeding service
    /// </summary>
    public class DatabaseInitializer
    {
        private readonly AppDbContext _context;

        public DatabaseInitializer(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Initialize database with tables and default data
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                // Create database if it doesn't exist
                await _context.Database.EnsureCreatedAsync();

                // Apply any pending migrations
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    await _context.Database.MigrateAsync();
                }

                // Seed default data
                await SeedDefaultDataAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Seed default admin user and sample data
        /// </summary>
        private async Task SeedDefaultDataAsync()
        {
            // Check if admin user exists
            if (! await _context.AdminUsers.AnyAsync())
            {
                // Create default admin user
                var adminUser = new AdminUser
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = UserRole.Admin,
                    CreatedDate = DateTime.Now
                };

                await _context. AdminUsers.AddAsync(adminUser);
                await _context.SaveChangesAsync();

                Console.WriteLine("Default admin user created: admin/admin123");
            }

            // Add sample departments (optional)
            // This can be extended based on requirements
        }

        /// <summary>
        /// Reset database (for testing purposes)
        /// </summary>
        public async Task ResetDatabaseAsync()
        {
            await _context. Database.EnsureDeletedAsync();
            await InitializeAsync();
        }
    }
}