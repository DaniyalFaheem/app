using System;
using FaceRecognitionAttendance. Models;

namespace FaceRecognitionAttendance.Services.Authentication
{
    /// <summary>
    /// Session management singleton
    /// </summary>
    public class SessionManager
    {
        private static SessionManager?  _instance;
        private static readonly object _lock = new object();

        private AdminUser? _currentUser;
        private DateTime _loginTime;
        private DateTime _lastActivity;
        private readonly int _timeoutMinutes = 30;

        private SessionManager() { }

        public static SessionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SessionManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public AdminUser? CurrentUser => _currentUser;

        public bool IsAuthenticated => _currentUser != null && IsSessionValid();

        public void StartSession(AdminUser user)
        {
            _currentUser = user;
            _loginTime = DateTime.Now;
            _lastActivity = DateTime.Now;
        }

        public void EndSession()
        {
            _currentUser = null;
        }

        public bool IsSessionValid()
        {
            if (_currentUser == null)
                return false;

            var elapsed = DateTime.Now - _lastActivity;
            return elapsed.TotalMinutes < _timeoutMinutes;
        }

        public void RefreshSession()
        {
            _lastActivity = DateTime.Now;
        }

        public TimeSpan GetSessionDuration()
        {
            return DateTime.Now - _loginTime;
        }

        public TimeSpan GetRemainingTime()
        {
            var elapsed = DateTime.Now - _lastActivity;
            var remaining = TimeSpan.FromMinutes(_timeoutMinutes) - elapsed;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }
}