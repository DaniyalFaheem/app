using System;
using System.Threading.Tasks;
using FaceRecognitionAttendance.Models;
using FaceRecognitionAttendance.Data.Repositories;

namespace FaceRecognitionAttendance.Services.Authentication
{
    /// <summary>
    /// Authentication service implementation
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAdminUserRepository _adminUserRepository;
        private readonly SessionManager _sessionManager;

        public AuthenticationService(IAdminUserRepository adminUserRepository)
        {
            _adminUserRepository = adminUserRepository ??  throw new ArgumentNullException(nameof(adminUserRepository));
            _sessionManager = SessionManager.Instance;
        }

        public AdminUser? CurrentUser => _sessionManager.CurrentUser;

        public bool IsAuthenticated => _sessionManager.IsAuthenticated;

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                // Get user by username
                var user = await _adminUserRepository.GetByUsernameAsync(username);
                
                if (user == null)
                    return false;

                // Verify password
                if (!BCrypt.Net.BCrypt. Verify(password, user.PasswordHash))
                    return false;

                // Update last login
                await _adminUserRepository.UpdateLastLoginAsync(user. Id);

                // Set current user and start session
                _sessionManager.StartSession(user);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                return false;
            }
        }

        public void Logout()
        {
            _sessionManager.EndSession();
        }

        public bool IsSessionValid()
        {
            return _sessionManager.IsSessionValid();
        }

        public void RefreshSession()
        {
            _sessionManager.RefreshSession();
        }
    }
}