using FaceRecognitionAttendance.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaceRecognitionAttendance.Data. Repositories
{
    /// <summary>
    /// Repository interface for User entity operations
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Get user by ID
        /// </summary>
        Task<User? > GetByIdAsync(int id);

        /// <summary>
        /// Get all users
        /// </summary>
        Task<List<User>> GetAllAsync();

        /// <summary>
        /// Get users by type (Student or Faculty)
        /// </summary>
        Task<List<User>> GetByTypeAsync(UserType userType);

        /// <summary>
        /// Get users by department
        /// </summary>
        Task<List<User>> GetByDepartmentAsync(string department);

        /// <summary>
        /// Get active users only
        /// </summary>
        Task<List<User>> GetActiveUsersAsync();

        /// <summary>
        /// Search users by name or ID
        /// </summary>
        Task<List<User>> SearchAsync(string searchTerm);

        /// <summary>
        /// Add new user
        /// </summary>
        Task<User> AddAsync(User user);

        /// <summary>
        /// Update existing user (for Edit feature)
        /// </summary>
        Task<User> UpdateAsync(User user);

        /// <summary>
        /// Delete user by ID
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Check if user with phone number exists
        /// </summary>
        Task<bool> ExistsByPhoneAsync(string phone);

        /// <summary>
        /// Get all face encodings for recognition
        /// </summary>
        Task<Dictionary<int, byte[]>> GetAllFaceEncodingsAsync();
    }
}