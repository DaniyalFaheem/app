using Microsoft.EntityFrameworkCore;
using FaceRecognitionAttendance.Models;
using System;
using System.Collections.Generic;
using System. Linq;
using System.Threading. Tasks;

namespace FaceRecognitionAttendance.Data. Repositories
{
    /// <summary>
    /// Repository implementation for AttendanceRecord entity operations
    /// </summary>
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;

        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AttendanceRecord?> GetByIdAsync(int id)
        {
            return await _context.AttendanceRecords
                .Include(a => a.User)
                . FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<AttendanceRecord>> GetAllAsync()
        {
            return await _context.AttendanceRecords
                .Include(a => a.User)
                .OrderByDescending(a => a.DateTime)
                .ToListAsync();
        }

        public async Task<List<AttendanceRecord>> GetByDateAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _context.AttendanceRecords
                .Include(a => a.User)
                .Where(a => a. DateTime >= startOfDay && a. DateTime < endOfDay)
                .OrderBy(a => a.DateTime)
                .ToListAsync();
        }

        public async Task<List<AttendanceRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date. AddDays(1);

            return await _context.AttendanceRecords
                .Include(a => a.User)
                . Where(a => a.DateTime >= start && a.DateTime < end)
                .OrderBy(a => a.DateTime)
                .ToListAsync();
        }

        public async Task<List<AttendanceRecord>> GetByUserIdAsync(int userId)
        {
            return await _context.AttendanceRecords
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a. DateTime)
                .ToListAsync();
        }

        public async Task<List<AttendanceRecord>> GetByUserAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date.AddDays(1);

            return await _context.AttendanceRecords
                .Include(a => a. User)
                .Where(a => a.UserId == userId && a.DateTime >= start && a. DateTime < end)
                .OrderBy(a => a.DateTime)
                .ToListAsync();
        }

        public async Task<List<AttendanceRecord>> GetByDepartmentAsync(string department, DateTime? date = null)
        {
            var query = _context.AttendanceRecords
                .Include(a => a.User)
                .Where(a => a.Department == department);

            if (date.HasValue)
            {
                var startOfDay = date.Value. Date;
                var endOfDay = startOfDay.AddDays(1);
                query = query.Where(a => a.DateTime >= startOfDay && a.DateTime < endOfDay);
            }

            return await query
                .OrderByDescending(a => a.DateTime)
                . ToListAsync();
        }

        public async Task<List<AttendanceRecord>> GetByTypeAsync(UserType userType, DateTime?  date = null)
        {
            var query = _context.AttendanceRecords
                .Include(a => a.User)
                .Where(a => a. Type == userType);

            if (date.HasValue)
            {
                var startOfDay = date.Value.Date;
                var endOfDay = startOfDay. AddDays(1);
                query = query.Where(a => a.DateTime >= startOfDay && a.DateTime < endOfDay);
            }

            return await query
                .OrderByDescending(a => a.DateTime)
                .ToListAsync();
        }

        public async Task<AttendanceRecord> AddAsync(AttendanceRecord record)
        {
            await _context.AttendanceRecords.AddAsync(record);
            await _context.SaveChangesAsync();
            return record;
        }

        public async Task<bool> HasAttendanceOnDateAsync(int userId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _context.AttendanceRecords
                .AnyAsync(a => a.UserId == userId && a.DateTime >= startOfDay && a.DateTime < endOfDay);
        }

        public async Task<AttendanceRecord?> GetLastAttendanceAsync(int userId)
        {
            return await _context.AttendanceRecords
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.DateTime)
                .FirstOrDefaultAsync();
        }

        public async Task<List<int>> GetPresentUserIdsAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _context.AttendanceRecords
                .Where(a => a.DateTime >= startOfDay && a.DateTime < endOfDay)
                .Select(a => a.UserId)
                . Distinct()
                .ToListAsync();
        }

        public async Task<int> GetAttendanceCountAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date.AddDays(1);

            // Count distinct dates (one attendance per day)
            return await _context.AttendanceRecords
                .Where(a => a.UserId == userId && a.DateTime >= start && a.DateTime < end)
                .Select(a => a.DateTime. Date)
                .Distinct()
                .CountAsync();
        }
    }
}