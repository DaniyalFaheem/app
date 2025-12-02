using FaceRecognitionAttendance. Models;
using System. Collections.Generic;
using System. Threading.Tasks;

namespace FaceRecognitionAttendance.Data.Repositories
{
    /// <summary>
    /// Repository interface for AdminUser entity operations
    /// </summary>
    public interface IAdminUserRepository
    {
        /// <summary>
        /// Get admin user by ID
        /// </summary>
        Task<AdminUser?> GetByIdAsync(int id);

        /// <summary>
        /// Get admin user by username
        /// </summary>
        Task<AdminUser?> GetByUsernameAsync(string username);

        /// <summary>
        /// Get all admin users
        /// </summary>
        Task<List<AdminUser>> GetAllAsync();

        /// <summary>
        /// Add new admin user
        /// </summary>
        Task<AdminUser> AddAsync(AdminUser adminUser);

        /// <summary>
        /// Update existing admin user
        /// </summary>
        Task<AdminUser> UpdateAsync(AdminUser adminUser);

        /// <summary>
        /// Delete admin user by ID
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Check if username exists
        /// </summary>
        Task<bool> UsernameExistsAsync(string username);

        /// <summary>
        /// Update last login timestamp
        /// </summary>
        Task UpdateLastLoginAsync(int id);

        /// <summary>
        /// Add audit log entry
        /// </summary>
        Task AddAuditLogAsync(AuditLog auditLog);

        /// <summary>
        /// Get audit logs for user
        /// </summary>
        Task<List<AuditLog>> GetAuditLogsAsync(int userId, int limit = 100);

        /// <summary>
        /// Get all audit logs
        /// </summary>
        Task<List<AuditLog>> GetAllAuditLogsAsync(int limit = 100);
    }
}