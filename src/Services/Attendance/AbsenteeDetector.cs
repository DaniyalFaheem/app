using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FaceRecognitionAttendance.Models;
using FaceRecognitionAttendance. Data.Repositories;

namespace FaceRecognitionAttendance. Services.Attendance
{
    /// <summary>
    /// Absentee detection service
    /// </summary>
    public class AbsenteeDetector
    {
        private readonly IUserRepository _userRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        public AbsenteeDetector(IUserRepository userRepository, IAttendanceRepository attendanceRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _attendanceRepository = attendanceRepository ?? throw new ArgumentNullException(nameof(attendanceRepository));
        }

        /// <summary>
        /// Detect absentees for a specific date
        /// </summary>
        public async Task<List<User>> DetectAbsenteesAsync(DateTime date, string?  department = null, UserType? userType = null)
        {
            // Get all registered users
            var allUsers = await _userRepository.GetActiveUsersAsync();

            // Apply filters
            if (! string.IsNullOrEmpty(department))
            {
                allUsers = allUsers.Where(u => u.Department == department).ToList();
            }

            if (userType.HasValue)
            {
                allUsers = allUsers.Where(u => u.UserType == userType.Value).ToList();
            }

            // Get present user IDs for the date
            var presentUserIds = await _attendanceRepository.GetPresentUserIdsAsync(date);

            // Find absentees
            var absentees = allUsers.Where(u => !presentUserIds.Contains(u.Id)).ToList();

            return absentees;
        }

        /// <summary>
        /// Get absentee statistics for a date range
        /// </summary>
        public async Task<Dictionary<int, int>> GetAbsenteeStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var statistics = new Dictionary<int, int>(); // userId -> absent count
            var allUsers = await _userRepository.GetActiveUsersAsync();
            var totalDays = (endDate. Date - startDate.Date).Days + 1;

            foreach (var user in allUsers)
            {
                var presentCount = await _attendanceRepository. GetAttendanceCountAsync(user.Id, startDate, endDate);
                var absentCount = totalDays - presentCount;
                statistics[user.Id] = absentCount;
            }

            return statistics;
        }
    }
}