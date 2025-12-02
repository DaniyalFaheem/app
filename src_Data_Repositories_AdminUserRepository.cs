using Microsoft.EntityFrameworkCore;
using FaceRecognitionAttendance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaceRecognitionAttendance.Data.Repositories
{
    /// <summary>
    /// Repository implementation for AdminUser entity operations
    /// </summary>
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly AppDbContext _context;

        public AdminUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminUser?> GetByIdAsync(int id)
        {
            return await _context.AdminUsers
                .Include(a => a. AuditLogs)
                . FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<AdminUser?> GetByUsernameAsync(string username)
        {
            return await _context.AdminUsers
                .FirstOrDefaultAsync(a => a. Username == username);
        }

        public async Task<List<AdminUser>> GetAllAsync()
        {
            return await _context. AdminUsers
                .OrderBy(a => a.Username)
                .ToListAsync();
        }

        public async Task<AdminUser> AddAsync(AdminUser adminUser)
        {
            await _context.AdminUsers.AddAsync(adminUser);
            await _context.SaveChangesAsync();
            return adminUser;
        }

        public async Task<AdminUser> UpdateAsync(AdminUser adminUser)
        {
            _context.AdminUsers.Update(adminUser);
            await _context. SaveChangesAsync();
            return adminUser;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var adminUser = await _context.AdminUsers.FindAsync(id);
            if (adminUser == null)
                return false;

            _context.AdminUsers.Remove(adminUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.AdminUsers
                .AnyAsync(a => a.Username == username);
        }

        public async Task UpdateLastLoginAsync(int id)
        {
            var adminUser = await _context.AdminUsers.FindAsync(id);
            if (adminUser != null)
            {
                adminUser.LastLogin = DateTime. Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddAuditLogAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AuditLog>> GetAuditLogsAsync(int userId, int limit = 100)
        {
            return await _context.AuditLogs
                .Include(a => a.AdminUser)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a. Timestamp)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetAllAuditLogsAsync(int limit = 100)
        {
            return await _context.AuditLogs
                .Include(a => a.AdminUser)
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .ToListAsync();
        }
    }
}