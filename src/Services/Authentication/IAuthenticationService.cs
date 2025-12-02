using FaceRecognitionAttendance.Models;
using System.Threading.Tasks;

namespace FaceRecognitionAttendance.Services.Authentication
{
    /// <summary>
    /// Authentication service interface
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Current logged-in admin user
        /// </summary>
        AdminUser? CurrentUser { get; }

        /// <summary>
        /// Check if user is authenticated
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Authenticate user with username and password
        /// </summary>
        Task<bool> LoginAsync(string username, string password);

        /// <summary>
        /// Logout current user
        /// </summary>
        void Logout();

        /// <summary>
        /// Check if session is still valid
        /// </summary>
        bool IsSessionValid();

        /// <summary>
        /// Refresh session timeout
        /// </summary>
        void RefreshSession();
    }
}