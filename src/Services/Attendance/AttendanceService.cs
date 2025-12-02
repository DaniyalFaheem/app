using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FaceRecognitionAttendance.Models;
using FaceRecognitionAttendance.Data.Repositories;

namespace FaceRecognitionAttendance.Services.Attendance
{
    /// <summary>
    /// Attendance service with cooldown management
    /// </summary>
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICsvExportService _csvExportService;
        private readonly Dictionary<int, DateTime> _cooldownTracker;
        private readonly int _cooldownMinutes = 5;

        public AttendanceService(IAttendanceRepository attendanceRepository, ICsvExportService csvExportService)
        {
            _attendanceRepository = attendanceRepository ?? throw new ArgumentNullException(nameof(attendanceRepository));
            _csvExportService = csvExportService ?? throw new ArgumentNullException(nameof(csvExportService));
            _cooldownTracker = new Dictionary<int, DateTime>();
        }

        public async Task<bool> LogAttendanceAsync(User user)
        {
            try
            {
                // Check cooldown
                if (await IsWithinCooldownAsync(user.Id))
                {
                    Console.WriteLine($"User {user.Name} is within cooldown period");
                    return false;
                }

                // Create attendance record
                var record = new AttendanceRecord
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Department = user.Department,
                    DateTime = DateTime.Now,
                    Type = user.UserType
                };

                // Save to database
                await _attendanceRepository. AddAsync(record);

                // Export to CSV
                await _csvExportService.ExportAttendanceAsync(record);

                // Update cooldown
                _cooldownTracker[user.Id] = DateTime.Now;

                Console.WriteLine($"Attendance logged for {user.Name} at {DateTime.Now:HH:mm:ss}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging attendance: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsWithinCooldownAsync(int userId)
        {
            // Check in-memory cooldown first
            if (_cooldownTracker.ContainsKey(userId))
            {
                var elapsed = DateTime.Now - _cooldownTracker[userId];
                if (elapsed. TotalMinutes < _cooldownMinutes)
                {
                    return true;
                }
            }

            // Check database for last attendance
            var lastAttendance = await _attendanceRepository.GetLastAttendanceAsync(userId);
            if (lastAttendance != null)
            {
                var elapsed = DateTime.Now - lastAttendance.DateTime;
                if (elapsed.TotalMinutes < _cooldownMinutes)
                {
                    _cooldownTracker[userId] = lastAttendance.DateTime;
                    return true;
                }
            }

            return false;
        }

        public async Task<TimeSpan?> GetRemainingCooldownAsync(int userId)
        {
            if (_cooldownTracker.ContainsKey(userId))
            {
                var elapsed = DateTime.Now - _cooldownTracker[userId];
                var remaining = TimeSpan.FromMinutes(_cooldownMinutes) - elapsed;
                
                if (remaining > TimeSpan.Zero)
                    return remaining;
            }

            var lastAttendance = await _attendanceRepository.GetLastAttendanceAsync(userId);
            if (lastAttendance != null)
            {
                var elapsed = DateTime.Now - lastAttendance. DateTime;
                var remaining = TimeSpan.FromMinutes(_cooldownMinutes) - elapsed;
                
                if (remaining > TimeSpan.Zero)
                    return remaining;
            }

            return null;
        }

        public void ResetCooldown(int userId)
        {
            if (_cooldownTracker.ContainsKey(userId))
            {
                _cooldownTracker. Remove(userId);
            }
        }
    }
}