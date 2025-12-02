using FaceRecognitionAttendance.Models;
using System;
using System.Threading.Tasks;

namespace FaceRecognitionAttendance.Services.Attendance
{
    /// <summary>
    /// Attendance service interface
    /// </summary>
    public interface IAttendanceService
    {
        /// <summary>
        /// Log attendance for a user
        /// </summary>
        Task<bool> LogAttendanceAsync(User user);

        /// <summary>
        /// Check if user is within cooldown period
        /// </summary>
        Task<bool> IsWithinCooldownAsync(int userId);

        /// <summary>
        /// Get remaining cooldown time for user
        /// </summary>
        Task<TimeSpan?> GetRemainingCooldownAsync(int userId);

        /// <summary>
        /// Reset cooldown for user (admin override)
        /// </summary>
        void ResetCooldown(int userId);
    }
}