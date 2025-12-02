using FaceRecognitionAttendance.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaceRecognitionAttendance.Data.Repositories
{
    /// <summary>
    /// Repository interface for AttendanceRecord entity operations
    /// </summary>
    public interface IAttendanceRepository
    {
        /// <summary>
        /// Get attendance record by ID
        /// </summary>
        Task<AttendanceRecord?> GetByIdAsync(int id);

        /// <summary>
        /// Get all attendance records
        /// </summary>
        Task<List<AttendanceRecord>> GetAllAsync();

        /// <summary>
        /// Get attendance records by date
        /// </summary>
        Task<List<AttendanceRecord>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Get attendance records by date range
        /// </summary>
        Task<List<AttendanceRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get attendance records for specific user
        /// </summary>
        Task<List<AttendanceRecord>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Get attendance records for user within date range
        /// </summary>
        Task<List<AttendanceRecord>> GetByUserAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get attendance records by department
        /// </summary>
        Task<List<AttendanceRecord>> GetByDepartmentAsync(string department, DateTime?  date = null);

        /// <summary>
        /// Get attendance records by user type
        /// </summary>
        Task<List<AttendanceRecord>> GetByTypeAsync(UserType userType, DateTime? date = null);

        /// <summary>
        /// Add new attendance record
        /// </summary>
        Task<AttendanceRecord> AddAsync(AttendanceRecord record);

        /// <summary>
        /// Check if user has attendance on specific date
        /// </summary>
        Task<bool> HasAttendanceOnDateAsync(int userId, DateTime date);

        /// <summary>
        /// Get last attendance for user
        /// </summary>
        Task<AttendanceRecord? > GetLastAttendanceAsync(int userId);

        /// <summary>
        /// Get present user IDs for a specific date
        /// </summary>
        Task<List<int>> GetPresentUserIdsAsync(DateTime date);

        /// <summary>
        /// Get attendance count for user in date range
        /// </summary>
        Task<int> GetAttendanceCountAsync(int userId, DateTime startDate, DateTime endDate);
    }
}