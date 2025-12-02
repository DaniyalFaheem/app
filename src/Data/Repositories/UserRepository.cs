using Microsoft.EntityFrameworkCore;
using FaceRecognitionAttendance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaceRecognitionAttendance.Data.Repositories
{
    /// <summary>
    /// Repository implementation for User entity operations
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context. Users
                .Include(u => u.AttendanceRecords)
                .FirstOrDefaultAsync(u => u. Id == id);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<List<User>> GetByTypeAsync(UserType userType)
        {
            return await _context. Users
                .Where(u => u.UserType == userType)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<List<User>> GetByDepartmentAsync(string department)
        {
            return await _context.Users
                . Where(u => u.Department == department)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<List<User>> GetActiveUsersAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<List<User>> SearchAsync(string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();
            
            return await _context.Users
                .Where(u => 
                    u.Name.ToLower().Contains(lowerSearchTerm) ||
                    u.Phone.Contains(searchTerm) ||
                    u.Department.ToLower().Contains(lowerSearchTerm) ||
                    u.Id.ToString().Contains(searchTerm))
                .OrderBy(u => u.Name)
                . ToListAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context. SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users. FindAsync(id);
            if (user == null)
                return false;

            _context. Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByPhoneAsync(string phone)
        {
            return await _context.Users. AnyAsync(u => u. Phone == phone);
        }

        public async Task<Dictionary<int, byte[]>> GetAllFaceEncodingsAsync()
        {
            return await _context.Users
                . Where(u => u.IsActive)
                .ToDictionaryAsync(u => u. Id, u => u.FaceEncoding);
        }
    }
}